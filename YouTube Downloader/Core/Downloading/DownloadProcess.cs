namespace YouTube.Downloader.Core.Downloading
{
    using System;
    using System.Diagnostics;

    internal class DownloadProcess
    {
        internal DownloadProcess(string arguments)
        {
            Process = new Process
            {
                EnableRaisingEvents = true,
                StartInfo = new ProcessStartInfo("Resources/youtube-dl.exe", arguments)
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true
                }
            };

            ProgressMonitor = new ProgressMonitor(Process);
        }

        internal event EventHandler Exited
        {
            add => Process.Exited += value;

            remove => Process.Exited -= value;
        }

        internal Process Process { get; }

        internal ProgressMonitor ProgressMonitor { get; }

        internal void Start()
        {
            Process.Start();
            ProgressMonitor.Run();
        }

        internal void Kill()
        {
            Process.Kill();
        }
    }
}