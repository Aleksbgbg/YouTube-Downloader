namespace YouTube.Downloader.Factories.Interfaces
{
    using YouTube.Downloader.Models;
    using YouTube.Downloader.ViewModels.Interfaces;

    internal interface IDownloadFactory
    {
        IDownloadViewModel MakeDownloadViewModel(IYouTubeVideoViewModel youTubeVideoViewModel);
    }
}