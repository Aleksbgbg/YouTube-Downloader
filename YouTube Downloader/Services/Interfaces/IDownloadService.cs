namespace YouTube.Downloader.Services.Interfaces
{
    using System.Collections.Generic;

    using YouTube.Downloader.Utilities.Downloading;

    internal interface IDownloadService
    {
        void QueueDownloads(IEnumerable<DownloadProcess> downloads);
    }
}