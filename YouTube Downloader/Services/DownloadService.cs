namespace YouTube.Downloader.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using YouTube.Downloader.Core.Downloading;
    using YouTube.Downloader.EventArgs;
    using YouTube.Downloader.Services.Interfaces;

    internal class DownloadService : IDownloadService
    {
        private const int MaxConcurrentDownloads = 3;

        private readonly LinkedList<Download> _downloadQueue = new LinkedList<Download>();

        private readonly List<Download> _currentDownloads = new List<Download>();

        public IEnumerable<Download> Downloads => _currentDownloads.Concat(_downloadQueue);

        public void QueueDownloads(IEnumerable<Download> downloads)
        {
            foreach (Download download in downloads)
            {
                _downloadQueue.AddLast(download);
            }

            while (_currentDownloads.Count < MaxConcurrentDownloads && _downloadQueue.Count > 0)
            {
                DownloadNext();
            }
        }

        public void ResumeDownloads(IEnumerable<Download> downloads)
        {
            foreach (Download download in downloads)
            {
                _downloadQueue.AddFirst(download);
                download.OnRequeued();
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

                Download download = _downloadQueue.First.Value;
                _downloadQueue.RemoveFirst();

                if (download.HasExited)
                {
                    continue;
                }

                _currentDownloads.Add(download);

                void DownloadFinished(object sender, DownloadFinishedEventArgs e)
                {
                    download.Finished -= DownloadFinished;

                    _currentDownloads.Remove(download);

                    DownloadNext();
                }

                void DownloadPaused(object sender, EventArgs e)
                {
                    _currentDownloads.Remove(download);

                    download.Finished -= DownloadFinished;
                    download.Paused -= DownloadPaused;

                    DownloadNext();
                }

                download.Finished += DownloadFinished;
                download.Paused += DownloadPaused;

                if (download.IsPaused)
                {
                    download.Resume();
                }
                else
                {
                    download.Start();
                }

                break;
            }
        }
    }
}