namespace YouTube.Downloader.Core.Downloading
{
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;

    using Caliburn.Micro;

    using YouTube.Downloader.EventArgs;
    using YouTube.Downloader.Models;
    using YouTube.Downloader.Models.Download;

    internal class DownloadProcess : MonitoredProcess
    {
        private static readonly ParameterMonitoring[] ParameterMonitorings =
        {
                new ParameterMonitoring("Progress", new Regex(@"^\[download] (?<ProgressPercentage>[ 1][ 0-9][0-9]\.[0-9])% of .*?(?<TotalDownloadSize>[\d\.]+)?(?<TotalDownloadSizeUnits>.iB) at +(?:(?<DownloadSpeed>.+)(?<DownloadSpeedUnits>.iB)\/s|Unknown speed)"), (currentValue, match) =>
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

                    Progress previousProgress = (Progress)currentValue ?? new Progress(0, 0, 0, 0);

                    int stage = previousProgress.Stage;

                    if (stage == 0 || progressPercentage >= 100 && previousProgress.DownloadPercentage < 100)
                    {
                        ++stage;
                    }

                    return new Progress(totalDownloadSize, progressPercentage, GetDownloadSpeed(), stage);
                }),
                new ParameterMonitoring("Destination", new Regex(@"^\[download] Destination: (?<Filename>.+)$"), (_, match) => match.Groups["Filename"].Value)
        };

        private readonly DownloadStatus _downloadStatus;

        internal DownloadProcess(DownloadStatus downloadStatus, YouTubeVideo youTubeVideo, Settings settings)
                :
                base("youtube-dl",
                     $"-o \"{settings.DownloadPath}\\%(title)s.%(ext)s\" -f {(settings.DownloadType == DownloadType.AudioVideo ? "bestvideo + bestaudio" : "bestaudio")} -- \"{youTubeVideo.Id}\"")
        {
            _downloadStatus = downloadStatus;
            YouTubeVideo = youTubeVideo;
        }

        internal YouTubeVideo YouTubeVideo { get; }

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

        private bool _hasExited;
        internal bool HasExited
        {
            get => _hasExited;

            private set
            {
                _hasExited = value;
                UpdateDownloadState();
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

        private protected override void OnStart()
        {
            ParameterMonitorings.Select(parameterMonitoring => parameterMonitoring.GetCopy()).Apply(ProcessMonitor.AddParameterMonitoring);

            ParameterMonitoring progressMonitoring = ProcessMonitor.ParameterMonitorings["Progress"];

            ProcessMonitor.Finished += ProcessMonitorFinished;
            progressMonitoring.ValueUpdated += ProgressUpdated;

            void ProcessMonitorFinished(object sender, EventArgs e)
            {
                ProcessMonitor.Finished -= ProcessMonitorFinished;
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

            HasStarted = true;
        }

        private protected override void OnExited(bool killed)
        {
            if (!killed)
            {
                DidComplete = true;
            }

            HasExited = true;
        }

        private void UpdateDownloadState()
        {
            if (HasExited)
            {
                _downloadStatus.DownloadState = DidComplete ? DownloadState.Completed : DownloadState.Exited;
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