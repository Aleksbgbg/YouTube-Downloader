namespace YouTube.Downloader.ViewModels.Interfaces
{
    using YouTube.Downloader.Core.Downloading;
    using YouTube.Downloader.Models.Download;

    internal interface IDownloadViewModel : IViewModelBase
    {
        IVideoViewModel VideoViewModel { get; }

        Download Download { get; }

        DownloadStatus DownloadStatus { get; }

        void Initialise(IVideoViewModel videoViewModel, Download download);
    }
}