namespace YouTube.Downloader.Services.Interfaces
{
    using System.Collections.Generic;

    using YouTube.Downloader.Core.Downloading;

    internal interface IDownloadService
    {
        void QueueDownloads(IEnumerable<Download> downloads);

        void SaveAndTerminateDownloads();
    }
}