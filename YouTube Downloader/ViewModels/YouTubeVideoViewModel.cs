namespace YouTube.Downloader.ViewModels
{
    using YouTube.Downloader.Models;
    using YouTube.Downloader.ViewModels.Interfaces;

    internal class YouTubeVideoViewModel : ViewModelBase, IYouTubeVideoViewModel
    {
        public YouTubeVideo Video { get; private set; }

        public void Initialise(YouTubeVideo video)
        {
            Video = video;
        }
    }
}