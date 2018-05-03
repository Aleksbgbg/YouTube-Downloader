namespace YouTube.Downloader.Factories.Interfaces
{
    using YouTube.Downloader.Core.Downloading;
    using YouTube.Downloader.Models;
    using YouTube.Downloader.ViewModels.Interfaces;

    internal interface IDownloadFactory
    {
        IDownloadViewModel MakeDownloadViewModel(IVideoViewModel videoViewModel);

        Download MakeDownload(YouTubeVideo youTubeVideo);
    }
}