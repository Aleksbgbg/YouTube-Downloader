namespace YouTube.Downloader.Core.Downloading
{
    using System;
    using System.Diagnostics;

    internal class MonitoredProcess
    {
        internal MonitoredProcess(string process, string arguments)
        {
            Process = new Process
            {
                EnableRaisingEvents = true,
                StartInfo = new ProcessStartInfo($"Resources/{process}.exe", arguments)
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true
                }
            };

            ProcessMonitor = new ProcessMonitor(Process);
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