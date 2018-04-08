namespace YouTube.Downloader.ViewModels.Interfaces
{
    using YouTube.Downloader.Models;

    internal interface IVideoViewModel : IViewModelBase
    {
        YouTubeVideo Video { get; }

        void Initialise(YouTubeVideo video);
    }
}