namespace YouTube.Downloader.ViewModels
{
    using YouTube.Downloader.Models;
    using YouTube.Downloader.ViewModels.Interfaces;

    internal class DownloadViewModel : ViewModelBase, IDownloadViewModel
    {
        public YouTubeVideo DownloadVideo { get; private set; }

        public void Initialise(YouTubeVideo downloadVideo)
        {
            DownloadVideo = downloadVideo;
        }
    }
}