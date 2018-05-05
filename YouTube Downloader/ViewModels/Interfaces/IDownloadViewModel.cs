namespace YouTube.Downloader.ViewModels.Interfaces
{
    using YouTube.Downloader.Core.Downloading;
    using YouTube.Downloader.Models;

    internal interface IDownloadViewModel : IViewModelBase
    {
        IVideoViewModel VideoViewModel { get; }

        Download Download { get; }

        DownloadState DownloadState { get; set; }

        DownloadProgress DownloadProgress { get; set; }

        void Initialise(IVideoViewModel videoViewModel, Download download);
    }
}