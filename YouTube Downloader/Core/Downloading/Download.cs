namespace YouTube.Downloader.Core.Downloading
{
    using System;
    using System.Diagnostics;
    using System.IO;

    using YouTube.Downloader.EventArgs;
    using YouTube.Downloader.Models;
    using YouTube.Downloader.Models.Download;

    internal class Download
    {
        private readonly string _processArguments;

        private readonly DownloadStatus _downloadStatus;

        private bool _isRequeued;

        internal Download(DownloadStatus downloadStatus, Settings settings, YouTubeVideo youTubeVideo)
        {
            string GetFormat()
            {
                switch (settings.OutputFormat)
                {
                    case OutputFormat.Auto:
                        switch (settings.DownloadType)
                        {
                            case DownloadType.AudioVideo:
                                return "bestvideo+bestaudio";

                            case DownloadType.Audio:
                                return "bestaudio";

                            default:
                                throw new InvalidOperationException("Download started with invalid settings.");
                        }

                    case OutputFormat.Mp4:
                        return "mp4/bestvideo+bestaudio";

                    case OutputFormat.Mp3:
                        return "mp3/bestaudio";

                    default:
                        throw new InvalidOperationException("Download started with invalid settings.");
                }
            }

            _downloadStatus = downloadStatus;
            _processArguments = $"-o \"{settings.DownloadPath}\\%(title)s.%(ext)s\" -f {GetFormat()} -- \"{youTubeVideo.Id}\"";
            YouTubeVideo = youTubeVideo;

            Finished += (sender, e) =>
            {
                if (!e.DidComplete || settings.OutputFormat == OutputFormat.Auto)
                {
                    return;
                }

                string destination = (string)DownloadProcess.ProcessMonitor.ParameterMonitorings["Destination"].Value;

                if (destination == null)
                {
                    return;
                }

                FileInfo destinationInfo = new FileInfo(destination);

                string expectedExtension = settings.OutputFormat == OutputFormat.Mp4 ? ".mp4" : ".mp3";

                if (destinationInfo.Extension != expectedExtension)
                {
                    Process.Start("Resources\\ffmpeg.exe", $"-i \"{destinationInfo.FullName}\" \"{Path.ChangeExtension(destinationInfo.FullName, expectedExtension)}\"");
                }
            };
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

        private DownloadProcess _downloadProcess;
        private DownloadProcess DownloadProcess
        {
            get => _downloadProcess;

            set
            {
                void DownloadProcessExited(object sender, EventArgs e)
                {
                    _downloadProcess.Exited -= DownloadProcessExited;

                    if (IsPaused || HasExited)
                    {
                        return;
                    }

                    DidComplete = true;
                    HasExited = true;
                }

                if (_downloadProcess != null)
                {
                    _downloadProcess.Exited -= DownloadProcessExited;
                }

                _downloadProcess = value;

                if (_downloadProcess == null)
                {
                    return;
                }

                _downloadProcess.Exited += DownloadProcessExited;

                ParameterMonitoring progressMonitoring = _downloadProcess.ProcessMonitor.ParameterMonitorings["Progress"];

                _downloadProcess.ProcessMonitor.Finished += ProcessMonitorFinished;
                progressMonitoring.ValueUpdated += ProgressUpdated;

                void ProcessMonitorFinished(object sender, EventArgs e)
                {
                    _downloadProcess.ProcessMonitor.Finished -= ProcessMonitorFinished;
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
            DownloadProcess = new DownloadProcess(_processArguments);
            DownloadProcess.Start();
        }

        private void KillProcess()
        {
            DownloadProcess.Kill();
            DownloadProcess = null;
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