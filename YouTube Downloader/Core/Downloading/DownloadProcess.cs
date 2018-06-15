namespace YouTube.Downloader.Core.Downloading
{
    using System;
    using System.Text.RegularExpressions;

    using YouTube.Downloader.EventArgs;
    using YouTube.Downloader.Models;
    using YouTube.Downloader.Models.Download;

    internal class DownloadProcess : MonitoredProcess
    {
        private static readonly ParameterMonitoring[] ParameterMonitorings =
        {
                new ParameterMonitoring("Progress", new Regex(@"^\[download] (?<ProgressPercentage>[ 1][ 0-9][0-9]\.[0-9])% of .*?(?<TotalDownloadSize>[\d\.]+)?(?<TotalDownloadSizeUnits>.iB) at +(?:(?<DownloadSpeed>.+)(?<DownloadSpeedUnits>.iB)\/s|Unknown speed)"), (currentValue, match) =>
                {
                    long? GetDownloadSpeed()
                    {
                        string downloadSpeed = match.Groups["DownloadSpeed"].Value;

                        if (downloadSpeed == string.Empty)
                        {
                            return null;
                        }

                        return DigitalStorageManager.GetBytes(double.Parse(downloadSpeed), match.Groups["DownloadSpeedUnits"].Value);
                    }

                    long totalDownloadSize = DigitalStorageManager.GetBytes(double.Parse(match.Groups["TotalDownloadSize"].Value), match.Groups["TotalDownloadSizeUnits"].Value);

                    double progressPercentage = double.Parse(match.Groups["ProgressPercentage"].Value);

                    Progress previousProgress = (Progress)currentValue ?? new Progress(0, 0, 0, 0);

                    int stage = previousProgress.Stage;

                    if (stage == 0 || progressPercentage >= 100 && previousProgress.DownloadPercentage < 100)
                    {
                        ++stage;
                    }

                    return new Progress(totalDownloadSize, progressPercentage, GetDownloadSpeed(), stage);
                }),
                new ParameterMonitoring("Destination", new Regex(@"^\[download] Destination: (.*)$|^\[download] (.*) has already been downloaded$|^\[ffmpeg] Merging formats into ""(.*)""$"), (_, match) =>
                {
                    if (match.Groups[3].Value != string.Empty)
                    {
                        return match.Groups[3].Value;
                    }

                    string firstMatch = match.Groups[1].Value;

                    return firstMatch == string.Empty ? match.Groups[2].Value : firstMatch;
                })
        };

        private readonly DownloadProgress _downloadProgress;

        internal DownloadProcess(DownloadProgress downloadProgress, YouTubeVideo youTubeVideo, Settings settings)
                :
                base("youtube-dl",
                     new DownloadArgumentBuilder
                     {
                         DownloadFolder = settings.DownloadPath,
                         DownloadType = settings.DownloadType,
                         VideoName = Regex.Replace(youTubeVideo.Title, @"[^\u0000-\u007F]+", string.Empty),
                         VideoId = youTubeVideo.Id,
                         OutputFormat = settings.OutputFormat
                     }.ToString(),
                     ParameterMonitorings)
        {
            _downloadProgress = downloadProgress;
        }

        private protected override void OnStart()
        {
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

                _downloadProgress.Stage = progress.Stage;

                if (progress.DownloadSpeed.HasValue)
                {
                    _downloadProgress.DownloadSpeed = progress.DownloadSpeed.Value;
                }

                _downloadProgress.ProgressPercentage = progress.DownloadPercentage;
                _downloadProgress.TotalDownloadSize = progress.TotalDownloadSize;
            }
        }
    }
}