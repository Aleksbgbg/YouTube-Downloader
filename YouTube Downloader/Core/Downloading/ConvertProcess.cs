namespace YouTube.Downloader.Core.Downloading
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text.RegularExpressions;

    using YouTube.Downloader.EventArgs;
    using YouTube.Downloader.Models.Download;

    internal class ConvertProcess : MonitoredProcess
    {
        private static readonly ParameterMonitoring[] ParameterMonitorings =
        {
            new ParameterMonitoring("Progress", new Regex(@"(?<Key>\w+)= *(?<Value>.+?)(?: |$)"), (dict, match) =>
            {
                Dictionary<string, string> keyValuePairs = (Dictionary<string, string>)dict;

                while (match != null)
                {
                    keyValuePairs[match.Groups["Key"].Value] = match.Groups["Value"].Value;

                    match = match.NextMatch();
                }

                return keyValuePairs;
            }, new Dictionary<string, string>())
        };

        private readonly string _filename;

        internal ConvertProcess(string filename, string newExtension, DownloadStatus downloadStatus)
                :
                base("ffmpeg",
                     $"-i \"{filename}\" \"{Path.ChangeExtension(filename, newExtension)}\"",
                     downloadStatus,
                     ParameterMonitorings)
        {
            _filename = filename;
        }

        private protected override void OnStart()
        {
            ParameterMonitoring progressMonitoring = ProcessMonitor.ParameterMonitorings["Progress"];

            ProcessMonitor.Finished += ProcessMonitorFinished;
            progressMonitoring.ValueUpdated += ProgressUpdated;

            void ProcessMonitorFinished(object sender, EventArgs e)
            {
                ProcessMonitor.Finished -= ProcessMonitorFinished;
                progressMonitoring.ValueUpdated -= ProgressUpdated;
            }

            void ProgressUpdated(object sender, ParameterUpdatedEventArgs e)
            {
            }
        }

        private protected override void OnExited(bool killed)
        {
            if (!killed)
            {
                File.Delete(_filename);
            }
        }
    }
}