namespace YouTube.Downloader.Services.Interfaces
{
    using System.Collections.Generic;

    using YouTube.Downloader.Core.Downloading;

    internal interface IDownloadService
    {
        IEnumerable<DownloadProcess> Downloads { get; }

        void QueueDownloads(IEnumerable<DownloadProcess> downloads);
    }
}