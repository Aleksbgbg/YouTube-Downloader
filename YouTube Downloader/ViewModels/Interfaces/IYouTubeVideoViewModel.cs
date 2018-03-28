namespace YouTube.Downloader.ViewModels.Interfaces
{
    using YouTube.Downloader.Models;

    internal interface IYouTubeVideoViewModel : IViewModelBase
    {
        YouTubeVideo Video { get; }

        void Initialise(YouTubeVideo video);
    }
}