namespace YouTube.Downloader.Core.Downloading
{
    using System;
    using System.Diagnostics;
    using System.Text.RegularExpressions;

    internal class DownloadProcess
    {
        internal DownloadProcess(string arguments)
        {
            Process = new Process
            {
                EnableRaisingEvents = true,
                StartInfo = new ProcessStartInfo("Resources/youtube-dl.exe", arguments)
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true
                }
            };

            {
                int stage = 0;
                double lastProgress = 0;

                ProcessMonitor = new ProcessMonitor(Process, new ParameterMonitoring[]
                {
                        new ParameterMonitoring("Progress", new Regex(@"^\[download] (?<ProgressPercentage>[ 1][ 0-9][0-9]\.[0-9])% of .*?(?<TotalDownloadSize>[\d\.]+)?(?<TotalDownloadSizeUnits>.iB) at +(?:(?<DownloadSpeed>.+)(?<DownloadSpeedUnits>.iB)\/s|Unknown speed)"), match =>
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
                        new ParameterMonitoring("Destination", new Regex(@"^\[download] Destination: (?<Filename>.+)$"), match => match.Groups["Filename"].Value)
                });
            }
        }

        internal event EventHandler Exited
        {
            add => Process.Exited += value;

            remove => Process.Exited -= value;
        }

        internal Process Process { get; }

        internal ProcessMonitor ProcessMonitor { get; }

        internal void Start()
        {
            Process.Start();
            ProcessMonitor.Run();
        }

        internal void Kill()
        {
            Process.Kill();
        }
    }
}