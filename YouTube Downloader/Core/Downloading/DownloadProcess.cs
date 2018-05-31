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
                new ParameterMonitoring("Destination", new Regex(@"^\[download] Destination: (.*)$|^\[download] (.*) has already been downloaded$"), (_, match) =>
                {
                    string firstMatch = match.Groups[1].Value;

                    return firstMatch == string.Empty ? match.Groups[2].Value : firstMatch;
                })
        };

        internal DownloadProcess(DownloadStatus downloadStatus, YouTubeVideo youTubeVideo, Settings settings)
                :
                base("youtube-dl",
                     $"-o \"{settings.DownloadPath}\\%(title)s.%(ext)s\" -f {(settings.DownloadType == DownloadType.AudioVideo ? "bestvideo+bestaudio" : "bestaudio")} -- \"{youTubeVideo.Id}\"",
                     downloadStatus)
        {
            YouTubeVideo = youTubeVideo;
        }

        internal YouTubeVideo YouTubeVideo { get; }

        internal bool CanKill => !HasExited;

        internal bool HasStarted { get; private set; }

        internal bool HasExited { get; private set; }

        internal bool DidComplete { get; private set; }

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

                DownloadStatus.DownloadProgress.Stage = progress.Stage;

                if (progress.DownloadSpeed.HasValue)
                {
                    DownloadStatus.DownloadProgress.DownloadSpeed = progress.DownloadSpeed.Value;
                }

                DownloadStatus.DownloadProgress.ProgressPercentage = progress.DownloadPercentage;
                DownloadStatus.DownloadProgress.TotalDownloadSize = progress.TotalDownloadSize;
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
    }
}