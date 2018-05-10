namespace YouTube.Downloader.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Caliburn.Micro;

    using YouTube.Downloader.Core.Downloading;
    using YouTube.Downloader.EventArgs;
    using YouTube.Downloader.Services.Interfaces;

    internal class DownloadService : IDownloadService
    {
        private const int MaxConcurrentDownloads = 3;

        private readonly Queue<Download> _downloadQueue = new Queue<Download>();

        private readonly List<Download> _currentDownloads = new List<Download>();

        private readonly IDataService _dataService;

        public DownloadService(IDataService dataService)
        {
            _dataService = dataService;
        }

        public IEnumerable<Download> Downloads => _currentDownloads.Concat(_downloadQueue);

        public void QueueDownloads(IEnumerable<Download> downloads)
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

                Download download = _downloadQueue.Dequeue();

                if (download.HasExited)
                {
                    continue;
                }

                _currentDownloads.Add(download);

                void DetachDownload(object sender, DownloadFinishedEventArgs e)
                {
                    download.Finished -= DetachDownload;

                    _currentDownloads.Remove(download);

                    DownloadNext();
                }

                download.Finished += DetachDownload;

                download.Start();
                break;
            }
        }
    }
}