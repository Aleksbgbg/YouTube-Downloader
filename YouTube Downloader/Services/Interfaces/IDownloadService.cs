namespace YouTube.Downloader.Services.Interfaces
{
    using System.Collections.Generic;

    using YouTube.Downloader.ViewModels.Interfaces;

    internal interface IDownloadService
    {
        void QueueDownloads(IEnumerable<IDownloadViewModel> downloads);

        void SaveAndTerminateDownloads();
    }
}