namespace YouTube.Downloader.Services
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    using YouTube.Downloader.Models;
    using YouTube.Downloader.Services.Interfaces;
    using YouTube.Downloader.ViewModels.Interfaces;

    internal class DownloadService : IDownloadService
    {
        private const int MaxConcurrentDownloads = 3;

        private readonly Queue<IDownloadViewModel> _downloadQueue = new Queue<IDownloadViewModel>();

        private int _concurrentDownloads;

        public void QueueDownloads(IEnumerable<IDownloadViewModel> downloads)
        {
            foreach (IDownloadViewModel download in downloads)
            {
                _downloadQueue.Enqueue(download);
            }

            for (int downloadsStarted = _concurrentDownloads; downloadsStarted < MaxConcurrentDownloads && _downloadQueue.Count > 0; ++downloadsStarted)
            {
                DownloadNext();
            }
        }

        private void DownloadNext()
        {
            if (_downloadQueue.Count == 0)
            {
                return;
            }

            IDownloadViewModel download = _downloadQueue.Dequeue();
            download.DownloadState = DownloadState.Downloading;

            Process downloadProcess = new Process
            {
                EnableRaisingEvents = true,
                StartInfo = new ProcessStartInfo("Resources/youtube-dl.exe", $"-o \"%(title)s.%(ext)s\" -f bestvideo+bestaudio \"{download.DownloadVideo.Id}\"")
                {
                    CreateNoWindow = true,
                    UseShellExecute = false
                }
            };

            void DownloadProcessExited(object sender, EventArgs e)
            {
                downloadProcess.Exited -= DownloadProcessExited;
                download.DownloadState = DownloadState.Completed;

                --_concurrentDownloads;

                DownloadNext();
            }

            downloadProcess.Exited += DownloadProcessExited;
            downloadProcess.Start();

            ++_concurrentDownloads;
        }
    }
}