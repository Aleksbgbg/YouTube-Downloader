namespace YouTube.Downloader.Helpers
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    using YouTube.Downloader.EventArgs;

    internal class ProgressMonitor
    {
        private static readonly Regex ProgressReportRegex = new Regex(@"^\[download] (?<ProgressPercentage>[ 1][ 0-9][0-9]\.[0-9])% of .*?(?<TotalDownloadSize>[\d\.]+)?(?<TotalDownloadSizeUnits>.iB) at  (?<DownloadSpeed>.+)(?<DownloadSpeedUnits>.iB?)\/s");

        private readonly Process _process;

        internal ProgressMonitor(Process process)
        {
            _process = process;
        }

        internal event EventHandler<ProgressUpdatedEventArgs> ProgressUpdated;

        internal event EventHandler FinishedMonitoring;

        internal void Run()
        {
            Task.Run(() =>
            {
                using (StreamReader progressReader = _process.StandardOutput)
                {
                    int stage = 0;
                    double lastProgress = double.MaxValue;

                    while (!progressReader.EndOfStream)
                    {
                        string line = progressReader.ReadLine();

                        if (line == null)
                        {
                            continue;
                        }

                        Match match = ProgressReportRegex.Match(line);

                        if (!match.Success)
                        {
                            continue;
                        }

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

                        long totalDownloadSize = GetBytes(double.Parse(match.Groups["TotalDownloadSize"].Value),
                                                          match.Groups["TotalDownloadSizeUnits"].Value);

                        long downloadSpeed = GetBytes(double.Parse(match.Groups["DownloadSpeed"].Value),
                                                      match.Groups["DownloadSpeedUnits"].Value);

                        double progressPercentage = double.Parse(match.Groups["ProgressPercentage"].Value);

                        if (progressPercentage < lastProgress)
                        {
                            ++stage;
                        }

                        ProgressUpdated?.Invoke(this, new ProgressUpdatedEventArgs(totalDownloadSize, progressPercentage, downloadSpeed, stage));

                        lastProgress = progressPercentage;
                    }
                }
            }).ContinueWith(task => FinishedMonitoring?.Invoke(this, EventArgs.Empty));
        }
    }
}