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

        internal ProcessMonitor(Process process, IEnumerable<ParameterMonitoring> parameterMonitorings)
        {
            _process = process;

            foreach (ParameterMonitoring parameterMonitoring in parameterMonitorings)
            {
                ParameterMonitorings[parameterMonitoring.Name] = parameterMonitoring;
            }
        }

        internal event EventHandler Finished;

        internal Dictionary<string, ParameterMonitoring> ParameterMonitorings { get; } = new Dictionary<string, ParameterMonitoring>();

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
                        Finished?.Invoke(this, EventArgs.Empty);
                    }
                }
            });
        }
    }
}