namespace YouTube.Downloader.ViewModels.Interfaces
{
    using YouTube.Downloader.Models;

    internal interface IDownloadViewModel : IViewModelBase
    {
        YouTubeVideo DownloadVideo { get; }

        void Initialise(YouTubeVideo downloadVideo);
    }
}