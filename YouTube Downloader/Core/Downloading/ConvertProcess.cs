namespace YouTube.Downloader.Core.Downloading
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text.RegularExpressions;

    using YouTube.Downloader.EventArgs;
    using YouTube.Downloader.Models;

    internal class ConvertProcess : MonitoredProcess
    {
        private static readonly ParameterMonitoring[] ParameterMonitorings =
        {
            new ParameterMonitoring("Progress", new Regex(@"(?<Key>\w+)= *(?<Value>.+?)(?: |$)"), (dict, match) =>
            {
                Dictionary<string, string> keyValuePairs = (Dictionary<string, string>)dict;

                while (match.Length != 0)
                {
                    keyValuePairs[match.Groups["Key"].Value] = match.Groups["Value"].Value;

                    match = match.NextMatch();
                }

                return keyValuePairs;
            }, new Dictionary<string, string>())
        };

        private readonly string _filename;

        private readonly ConvertProgress _convertProgress;

        internal ConvertProcess(string filename, string newExtension, ConvertProgress convertProgress)
                :
                base("ffmpeg",
                     $"-i \"{filename}\" \"{Path.ChangeExtension(filename, newExtension)}\"",
                     ParameterMonitorings)
        {
            _filename = filename;
            _convertProgress = convertProgress;
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
                Dictionary<string, string> keyValuePairs = (Dictionary<string, string>)e.NewValue;

                if (keyValuePairs.TryGetValue("size", out string size))
                {
                    Match sizeMatch = Regex.Match(size, @"(?<Size>\d+)(?<Units>.+)");
                    _convertProgress.ConvertedBytes = DigitalStorageManager.GetBytes(double.Parse(sizeMatch.Groups["Size"].Value), sizeMatch.Groups["Units"].Value);
                }

                if (keyValuePairs.TryGetValue("bitrate", out string bitrate))
                {
                    Match bitrateMatch = Regex.Match(bitrate, @"(?<Size>\d+(?:\.\d+)?)(?<Units>.+)its\/s");
                    _convertProgress.Bitrate = DigitalStorageManager.GetBytes(double.Parse(bitrateMatch.Groups["Size"].Value), bitrateMatch.Groups["Units"].Value);
                }
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