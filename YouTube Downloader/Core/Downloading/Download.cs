namespace YouTube.Downloader.Core.Downloading
{
    using System;
    using System.IO;
    using System.Text.RegularExpressions;

    using Caliburn.Micro;

    using YouTube.Downloader.EventArgs;
    using YouTube.Downloader.Models;
    using YouTube.Downloader.Models.Download;
    using YouTube.Downloader.Services.Interfaces;

    internal class Download
    {
        private static readonly Settings Settings = IoC.Get<ISettingsService>().Settings;

        private readonly string _processArguments;

        private readonly DownloadStatus _downloadStatus;

        private bool _isRequeued;

        private MonitoredProcess _monitoredProcess;

        internal Download(DownloadStatus downloadStatus, YouTubeVideo youTubeVideo)
        {
            string GetFormat()
            {
                switch (Settings.OutputFormat)
                {
                    case OutputFormat.Auto:
                        switch (Settings.DownloadType)
                        {
                            case DownloadType.AudioVideo:
                                return "bestvideo+bestaudio";

                            case DownloadType.Audio:
                                return "bestaudio";

                            default:
                                throw new InvalidOperationException("Download started with invalid Settings.");
                        }

                    case OutputFormat.Mp4:
                        return "mp4/bestvideo+bestaudio";

                    case OutputFormat.Mp3:
                        return "mp3/bestaudio";

                    default:
                        throw new InvalidOperationException("Download started with invalid Settings.");
                }
            }

            _downloadStatus = downloadStatus;
            _processArguments = $"-o \"{Settings.DownloadPath}\\%(title)s.%(ext)s\" -f {GetFormat()} -- \"{youTubeVideo.Id}\"";
            YouTubeVideo = youTubeVideo;
        }

        internal event EventHandler<DownloadFinishedEventArgs> Finished;

        internal event EventHandler Paused;

        internal event EventHandler Resumed;

        internal YouTubeVideo YouTubeVideo { get; }

        internal bool CanPause => HasStarted && !HasExited && !IsPaused;

        internal bool CanResume => HasStarted && !HasExited && IsPaused && !_isRequeued;

        internal bool CanKill => !HasExited;

        private bool _hasStarted;
        internal bool HasStarted
        {
            get => _hasStarted;

            private set
            {
                _hasStarted = value;
                UpdateDownloadState();
            }
        }

        private bool _isPaused;
        internal bool IsPaused
        {
            get => _isPaused;

            private set
            {
                _isPaused = value;
                UpdateDownloadState();
            }
        }

        private bool _hasExited;
        internal bool HasExited
        {
            get => _hasExited;

            private set
            {
                _hasExited = value;
                UpdateDownloadState();

                if (_hasExited)
                {
                    Finished?.Invoke(this, new DownloadFinishedEventArgs(DidComplete));
                }
            }
        }

        private bool _didComplete;
        internal bool DidComplete
        {
            get => _didComplete;

            private set
            {
                _didComplete = value;
                UpdateDownloadState();
            }
        }

        internal void Start()
        {
            if (HasStarted)
            {
                throw new InvalidOperationException("Cannot start an already started download.");
            }

            HasStarted = true;
            GenerateAndStartProcess();
        }

        internal void Kill()
        {
            if (!HasStarted || IsPaused)
            {
                HasExited = true;
                return;
            }

            if (HasExited)
            {
                throw new InvalidOperationException("Cannot kill a download which has already been killed.");
            }

            HasExited = true;
            KillProcess();
        }

        internal void Pause()
        {
            if (!HasStarted)
            {
                throw new InvalidOperationException("Cannot pause a download which has not been started.");
            }

            if (HasExited)
            {
                throw new InvalidOperationException("Cannot pause a download which has already been killed.");
            }

            if (IsPaused)
            {
                throw new InvalidOperationException("Cannot pause a paused download.");
            }

            IsPaused = true;
            KillProcess();
            Paused?.Invoke(this, EventArgs.Empty);
        }

        internal void Resume()
        {
            if (!IsPaused)
            {
                throw new InvalidOperationException("Cannot resume a download which has not been paused.");
            }

            _isRequeued = false;
            IsPaused = false;
            GenerateAndStartProcess();
            Resumed?.Invoke(this, EventArgs.Empty);
        }

        internal void OnRequeued()
        {
            _downloadStatus.DownloadState = DownloadState.Queued;
            _isRequeued = true;
        }

        private void GenerateAndStartProcess()
        {
            _monitoredProcess = new MonitoredProcess("youtube-dl", _processArguments);

            void DownloadProcessExited(object sender, EventArgs e)
            {
                string path = null;

                _monitoredProcess.Exited -= DownloadProcessExited;

                if (IsPaused || HasExited)
                {
                    return;
                }

                if (Settings.OutputFormat != OutputFormat.Auto)
                {
                    string destination = (string)_monitoredProcess.ProcessMonitor.ParameterMonitorings["Destination"].Value;

                    if (destination == null)
                    {
                        return;
                    }

                    FileInfo destinationInfo = new FileInfo(destination);

                    string expectedExtension = Settings.OutputFormat == OutputFormat.Mp4 ? ".mp4" : ".mp3";

                    if (destinationInfo.Extension != expectedExtension)
                    {
                        _monitoredProcess = new MonitoredProcess("ffmpeg", $"-i \"{destinationInfo.FullName}\" \"{Path.ChangeExtension(destinationInfo.FullName, expectedExtension)}\"");

                        path = destinationInfo.FullName;

                        void ConversionProcessExited(object _, EventArgs args)
                        {
                            _monitoredProcess.Exited -= ConversionProcessExited;

                            File.Delete(path);

                            DidComplete = true;
                            HasExited = true;
                        }

                        _monitoredProcess.Exited += ConversionProcessExited;
                        _monitoredProcess.Start();

                        _downloadStatus.DownloadState = DownloadState.Converting;

                        return;
                    }
                }

                DidComplete = true;
                HasExited = true;
            }

            _monitoredProcess.Exited += DownloadProcessExited;

            {
                int stage = 0;
                double lastProgress = 0;

                foreach (ParameterMonitoring parameterMonitoring in new ParameterMonitoring[]
                {
                        new ParameterMonitoring("Progress", ProgressMonitoringRegexes.ProgressRegex, match =>
                        {
                            long GetBytes(double size, string units)
                            {
                                int GetMultiplier()
                                {
                                    switch (units)
                                    {
                                        case "KiB":
                                            return 1024;

                                        case "MiB":
                                            return 1024 * 1024;

                                        case "GiB":
                                            return 1024 * 1024 * 1024;

                                        default:
                                            throw new InvalidOperationException("Invalid units for multiplier.");
                                    }
                                }

                                return (long)(size * GetMultiplier());
                            }

                            long? GetDownloadSpeed()
                            {
                                string downloadSpeed = match.Groups["DownloadSpeed"].Value;

                                if (downloadSpeed == string.Empty)
                                {
                                    return null;
                                }

                                return GetBytes(double.Parse(downloadSpeed), match.Groups["DownloadSpeedUnits"].Value);
                            }

                            long totalDownloadSize = GetBytes(double.Parse(match.Groups["TotalDownloadSize"].Value), match.Groups["TotalDownloadSizeUnits"].Value);

                            double progressPercentage = double.Parse(match.Groups["ProgressPercentage"].Value);

                            if (stage == 0 || progressPercentage >= 100 && lastProgress < 100)
                            {
                                ++stage;
                            }

                            lastProgress = progressPercentage;

                            return new Progress(totalDownloadSize, progressPercentage, GetDownloadSpeed(), stage);
                        }),
                        new ParameterMonitoring("Destination", ProgressMonitoringRegexes.DestinationRegex, match => match.Groups["Filename"].Value)
                })
                {
                    _monitoredProcess.ProcessMonitor.AddParameterMonitoring(parameterMonitoring);
                }
            }

            ParameterMonitoring progressMonitoring = _monitoredProcess.ProcessMonitor.ParameterMonitorings["Progress"];

            _monitoredProcess.ProcessMonitor.Finished += ProcessMonitorFinished;
            progressMonitoring.ValueUpdated += ProgressUpdated;

            void ProcessMonitorFinished(object sender, EventArgs e)
            {
                _monitoredProcess.ProcessMonitor.Finished -= ProcessMonitorFinished;
                progressMonitoring.ValueUpdated -= ProgressUpdated;
            }

            void ProgressUpdated(object sender, ParameterUpdatedEventArgs e)
            {
                Progress progress = (Progress)e.NewValue;

                _downloadStatus.DownloadProgress.Stage = progress.Stage;

                if (progress.DownloadSpeed.HasValue)
                {
                    _downloadStatus.DownloadProgress.DownloadSpeed = progress.DownloadSpeed.Value;
                }

                _downloadStatus.DownloadProgress.ProgressPercentage = progress.DownloadPercentage;
                _downloadStatus.DownloadProgress.TotalDownloadSize = progress.TotalDownloadSize;
            }

            _monitoredProcess.Start();
        }

        private void KillProcess()
        {
            _monitoredProcess.Kill();
            _monitoredProcess = null;
        }

        private void UpdateDownloadState()
        {
            if (HasExited)
            {
                _downloadStatus.DownloadState = DidComplete ? DownloadState.Completed : DownloadState.Exited;
            }
            else if (IsPaused)
            {
                _downloadStatus.DownloadState = DownloadState.Paused;
            }
            else if (HasStarted)
            {
                _downloadStatus.DownloadState = DownloadState.Downloading;
            }
            else
            {
                _downloadStatus.DownloadState = DownloadState.Queued;
            }
        }
    }
}