namespace YouTube.Downloader.Factories.Interfaces
{
    using YouTube.Downloader.Models;
    using YouTube.Downloader.ViewModels.Interfaces;

    internal interface IYouTubeFactory
    {
        IYouTubeVideoViewModel MakeVideoViewModel(YouTubeVideo video);
    }
}