namespace YouTube.Downloader.Core.Downloading
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    using YouTube.Downloader.EventArgs;

    internal class ProgressMonitor
    {
        private static readonly Regex ProgressReportRegex = new Regex(@"^\[download] (?<ProgressPercentage>[ 1][ 0-9][0-9]\.[0-9])% of .*?(?<TotalDownloadSize>[\d\.]+)?(?<TotalDownloadSizeUnits>.iB) at +(?:(?<DownloadSpeed>.+)(?<DownloadSpeedUnits>.iB)\/s|Unknown speed)");

        private readonly Process _process;

        private int _stage;

        internal ProgressMonitor(Process process)
        {
            _process = process;
        }

        internal event EventHandler<ProgressUpdatedEventArgs> ProgressUpdated;

        internal event EventHandler FinishedMonitoring;

        internal void Run()
        {
            RunMonitoringThread();
        }

        internal void MonitorDownload(EventHandler<ProgressUpdatedEventArgs> eventHandler)
        {
            ProgressUpdated += eventHandler;

            void ProgressMonitorFinishedMonitoring(object sender, EventArgs e)
            {
                FinishedMonitoring -= ProgressMonitorFinishedMonitoring;
                ProgressUpdated -= eventHandler;
            }

            FinishedMonitoring += ProgressMonitorFinishedMonitoring;
        }

        private void RunMonitoringThread()
        {
            Task.Run(() =>
            {
                using (StreamReader progressReader = _process.StandardOutput)
                {
                    try
                    {
                        while (!progressReader.EndOfStream)
                        {
                            string line = progressReader.ReadLine();

                            if (line == null) continue;

                            Match match = ProgressReportRegex.Match(line);

                            if (!match.Success) continue;

                            ReportProgress(match);
                        }
                    }
                    finally
                    {
                        FinishedMonitoring?.Invoke(this, EventArgs.Empty);
                    }
                }
            });
        }

        private void ReportProgress(Match match)
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

            long totalDownloadSize = GetBytes(double.Parse(match.Groups["TotalDownloadSize"].Value),
                                              match.Groups["TotalDownloadSizeUnits"].Value);

            double progressPercentage = double.Parse(match.Groups["ProgressPercentage"].Value);

            if (_stage == 0 || progressPercentage == 100)
            {
                ++_stage;
            }

            ProgressUpdated?.Invoke(this, new ProgressUpdatedEventArgs(totalDownloadSize, progressPercentage, GetDownloadSpeed(), _stage));
        }
    }
}