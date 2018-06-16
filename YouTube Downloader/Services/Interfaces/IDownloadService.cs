namespace YouTube.Downloader.Services.Interfaces
{
    using System.Collections.Generic;

    using YouTube.Downloader.Utilities.Processing;

    internal interface IDownloadService
    {
        void QueueDownloads(IEnumerable<DownloadProcess> downloads);
    }
}