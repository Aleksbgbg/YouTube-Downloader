namespace YouTube.Downloader.Utilities.Processing
{
    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    internal class ProcessMonitor
    {
        private readonly Process _process;

        internal ProcessMonitor(Process process)
        {
            _process = process;
        }

        internal event EventHandler Finished;

        internal ConcurrentDictionary<string, ParameterMonitoring> ParameterMonitorings { get; } = new ConcurrentDictionary<string, ParameterMonitoring>();

        private bool _hasFinished;
        internal bool HasFinished
        {
            get => _hasFinished;

            private set
            {
                _hasFinished = value;

                if (_hasFinished)
                {
                    Finished?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        internal void AddParameterMonitoring(ParameterMonitoring parameterMonitoring)
        {
            ParameterMonitorings[parameterMonitoring.Name] = parameterMonitoring;
        }

        internal void Run()
        {
            RunMonitoringThread(_process.StandardOutput);
            RunMonitoringThread(_process.StandardError);
        }

        private void RunMonitoringThread(StreamReader reader)
        {
            Task.Run(() =>
            {
                try
                {
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();

                        if (line == null) continue;

#if DEBUG
                        Console.WriteLine(line);
#endif

                        foreach (ParameterMonitoring parameterMonitoring in ParameterMonitorings.Values)
                        {
                            Match regexMatch = parameterMonitoring.Regex.Match(line);

                            if (regexMatch.Success)
                            {
                                parameterMonitoring.Update(regexMatch);
                            }
                        }
                    }
                }
                finally
                {
                    CheckFinished();
                }
            });
        }

        private void CheckFinished()
        {
            if (_process.StandardOutput.EndOfStream && _process.StandardError.EndOfStream)
            {
                HasFinished = true;
            }
        }
    }
}