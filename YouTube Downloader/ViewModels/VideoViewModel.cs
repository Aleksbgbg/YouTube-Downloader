namespace YouTube.Downloader.ViewModels
{
    using YouTube.Downloader.Models;
    using YouTube.Downloader.ViewModels.Interfaces;

    internal class VideoViewModel : ViewModelBase, IVideoViewModel
    {
        public YouTubeVideo Video { get; private set; }

        public void Initialise(YouTubeVideo video)
        {
            Video = video;
        }
    }
}