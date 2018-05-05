namespace YouTube.Downloader.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Caliburn.Micro;

    using YouTube.Downloader.Core.Downloading;
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

        public void QueueDownloads(IEnumerable<Download> downloads)
        {
            downloads.Apply(_downloadQueue.Enqueue);

            for (int downloadsStarted = _currentDownloads.Count; downloadsStarted < MaxConcurrentDownloads && _downloadQueue.Count > 0; ++downloadsStarted)
            {
                DownloadNext();
            }
        }

        public void SaveAndTerminateDownloads()
        {
            Download[] downloads = _currentDownloads.ToArray();

            _dataService.Save(downloads.Concat(_downloadQueue).Select(download => download.YouTubeVideo), "Downloads");

            downloads.Apply(download => download.Kill());
        }

        private void DownloadNext()
        {
            if (_downloadQueue.Count == 0)
            {
                return;
            }

            Download download = _downloadQueue.Dequeue();
            _currentDownloads.Add(download);

            void DetachDownload(object sender, EventArgs e)
            {
                download.Exited -= DetachDownload;

                _currentDownloads.Remove(download);
                DownloadNext();
            }

            download.Exited += DetachDownload;

            download.Start();
        }
    }
}