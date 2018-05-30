namespace YouTube.Downloader.Services
{
    using System;
    using System.Collections.Generic;

    using Caliburn.Micro;

    using YouTube.Downloader.Core.Downloading;
    using YouTube.Downloader.Services.Interfaces;

    internal class DownloadService : IDownloadService
    {
        private const int MaxConcurrentDownloads = 3;

        private readonly Queue<DownloadProcess> _downloadQueue = new Queue<DownloadProcess>();

        private readonly List<DownloadProcess> _currentDownloads = new List<DownloadProcess>();

        public void QueueDownloads(IEnumerable<DownloadProcess> downloads)
        {
            downloads.Apply(_downloadQueue.Enqueue);

            while (_currentDownloads.Count < MaxConcurrentDownloads && _downloadQueue.Count > 0)
            {
                DownloadNext();
            }
        }

        private void DownloadNext()
        {
            while (true)
            {
                if (_downloadQueue.Count == 0)
                {
                    return;
                }

                DownloadProcess downloadProcess = _downloadQueue.Dequeue();

                if (downloadProcess.HasExited)
                {
                    continue;
                }

                _currentDownloads.Add(downloadProcess);

                void DownloadFinished(object sender, EventArgs e)
                {
                    downloadProcess.Exited -= DownloadFinished;

                    _currentDownloads.Remove(downloadProcess);

                    DownloadNext();
                }

                downloadProcess.Exited += DownloadFinished;

                downloadProcess.Start();

                break;
            }
        }
    }
}