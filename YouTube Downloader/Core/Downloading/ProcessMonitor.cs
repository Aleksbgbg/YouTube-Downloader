namespace YouTube.Downloader.Core.Downloading
{
    using System;
    using System.Collections.Generic;
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

        internal Dictionary<string, ParameterMonitoring> ParameterMonitorings { get; } = new Dictionary<string, ParameterMonitoring>();

        internal void AddParameterMonitoring(ParameterMonitoring parameterMonitoring)
        {
            lock (ParameterMonitorings)
            {
                ParameterMonitorings[parameterMonitoring.Name] = parameterMonitoring;
            }
        }

        internal void Run()
        {
            RunMonitoringThread();
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

                            lock (ParameterMonitorings)
                            {
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
                    }
                    finally
                    {
                        Finished?.Invoke(this, EventArgs.Empty);
                    }
                }
            });
        }
    }
}