namespace YouTube.Downloader.Core.Downloading
{
    using System;
    using System.Diagnostics;

    internal class MonitoredProcess
    {
        private readonly Process _process;

        internal MonitoredProcess(string process, string arguments)
        {
            _process = new Process
            {
                EnableRaisingEvents = true,
                StartInfo = new ProcessStartInfo($"Resources/{process}.exe", arguments)
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true
                }
            };

            ProcessMonitor = new ProcessMonitor(_process);
        }

        internal event EventHandler Exited
        {
            add => _process.Exited += value;

            remove => _process.Exited -= value;
        }

        private protected ProcessMonitor ProcessMonitor { get; }

        internal virtual void Start()
        {
            _process.Start();
            ProcessMonitor.Run();
        }

        internal virtual void Kill()
        {
            _process.Kill();
        }
    }
}