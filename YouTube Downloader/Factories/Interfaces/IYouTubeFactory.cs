namespace YouTube.Downloader.Factories.Interfaces
{
    using YouTube.Downloader.Models;
    using YouTube.Downloader.ViewModels.Interfaces;

    internal interface IYouTubeFactory
    {
        IVideoViewModel MakeVideoViewModel(YouTubeVideo video);

        IMatchedVideoViewModel MakeMatchedVideoViewModel(YouTubeVideo video);

        IMatchedVideoViewModel MakeMatchedVideoViewModel(IVideoViewModel videoViewModel);
    }
}