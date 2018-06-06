namespace YouTube.Downloader.Core.Downloading
{
    using System;
    using System.Diagnostics;

    internal abstract class MonitoredProcess
    {
        private readonly ParameterMonitoring[] _parameterMonitorings;

        private readonly Process _process;

        private protected MonitoredProcess(string process, string arguments)
                : this(process, arguments, new ParameterMonitoring[] { })
        {
        }

        private protected MonitoredProcess(string process, string arguments, ParameterMonitoring[] parameterMonitorings)
        {
            _process = new Process
            {
                EnableRaisingEvents = true,
                StartInfo = new ProcessStartInfo($"Resources/{process}.exe", arguments)
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                }
            };

            _parameterMonitorings = parameterMonitorings;

            ProcessMonitor = new ProcessMonitor(_process);

            Exited += (sender, e) => OnExited(Killed);
        }

        internal event EventHandler Started;

        internal event EventHandler Exited
        {
            add => _process.Exited += value;

            remove => _process.Exited -= value;
        }

        internal bool Killed { get; private set; }

        internal ProcessMonitor ProcessMonitor { get; }

        internal void Start()
        {
            foreach (ParameterMonitoring parameterMonitoring in _parameterMonitorings)
            {
                ProcessMonitor.AddParameterMonitoring(parameterMonitoring.GetCopy());
            }

            _process.Start();
            ProcessMonitor.Run();

            Started?.Invoke(this, EventArgs.Empty);

            OnStart();
        }

        internal void Kill()
        {
            Killed = true;

            _process.Kill();
        }

        private protected virtual void OnStart()
        {
        }

        private protected virtual void OnExited(bool killed)
        {
        }
    }
}