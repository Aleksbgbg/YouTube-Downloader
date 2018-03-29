namespace YouTube.Downloader.ViewModels.Interfaces
{
    using YouTube.Downloader.Models;

    internal interface IYouTubeVideoViewModel : IViewModelBase
    {
        bool IsSelected { get; set; }

        YouTubeVideo Video { get; }

        void Initialise(YouTubeVideo video);
    }
}