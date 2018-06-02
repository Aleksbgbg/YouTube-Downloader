namespace YouTube.Downloader.Core.Downloading
{
    using System;
    using System.Diagnostics;

    using YouTube.Downloader.Models.Download;

    internal abstract class MonitoredProcess
    {
        private readonly ParameterMonitoring[] _parameterMonitorings;

        private readonly Process _process;

        private bool _calledKill;

        internal MonitoredProcess(string process, string arguments, DownloadStatus downloadStatus)
                : this(process, arguments, downloadStatus, new ParameterMonitoring[] { })
        {
        }

        internal MonitoredProcess(string process, string arguments, DownloadStatus downloadStatus, ParameterMonitoring[] parameterMonitorings)
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

            DownloadStatus = downloadStatus;
            _parameterMonitorings = parameterMonitorings;

            ProcessMonitor = new ProcessMonitor(_process);

            Exited += (sender, e) =>
            {
                if (!_calledKill)
                {
                    OnExited(false);
                }
            };
        }

        internal event EventHandler Exited
        {
            add => _process.Exited += value;

            remove => _process.Exited -= value;
        }

        internal DownloadStatus DownloadStatus { get; }

        internal ProcessMonitor ProcessMonitor { get; }

        internal void Start()
        {
            foreach (ParameterMonitoring parameterMonitoring in _parameterMonitorings)
            {
                ProcessMonitor.AddParameterMonitoring(parameterMonitoring.GetCopy());
            }

            _process.Start();
            ProcessMonitor.Run();

            OnStart();
        }

        internal void Kill()
        {
            _calledKill = true;

            _process.Kill();

            OnExited(true);
        }

        private protected virtual void OnStart()
        {
        }

        private protected virtual void OnExited(bool killed)
        {
        }
    }
}