namespace YouTube.Downloader.Core.Downloading
{
    using System;
    using System.Diagnostics;

    internal abstract class MonitoredProcess
    {
        private readonly ParameterMonitoring[] _parameterMonitorings;

        private readonly Process _process;

        private bool _hasStarted;

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

            _process.Exited += delegate
            {
                if (ProcessMonitor.HasFinished)
                {
                    OnExited();
                    return;
                }

                ProcessMonitor.Finished += (sender, e) => OnExited();
            };
        }

        internal event EventHandler Started;

        internal event EventHandler Exited;

        internal bool Killed { get; private set; }

        internal ProcessMonitor ProcessMonitor { get; }

        internal void Start()
        {
            foreach (ParameterMonitoring parameterMonitoring in _parameterMonitorings)
            {
                ProcessMonitor.AddParameterMonitoring(parameterMonitoring.GetCopy());
            }

            _process.Start();

            _hasStarted = true;

            ProcessMonitor.Run();

            Started?.Invoke(this, EventArgs.Empty);

            OnStart();
        }

        internal void Kill()
        {
            Killed = true;

            if (_hasStarted)
            {
                _process.Kill();
            }
            else
            {
                OnExited();
            }
        }

        private protected virtual void OnStart()
        {
        }

        private protected virtual void OnExited(bool killed)
        {
        }

        private void OnExited()
        {
            OnExited(Killed);
            Exited?.Invoke(this, EventArgs.Empty);
        }
    }
}