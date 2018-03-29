namespace YouTube.Downloader.ViewModels
{
    using System;
    using System.Threading.Tasks;

    using YouTube.Downloader.Models;
    using YouTube.Downloader.ViewModels.Interfaces;

    internal class DownloadViewModel : ViewModelBase, IDownloadViewModel
    {
        public YouTubeVideo DownloadVideo { get; private set; }

        public void Initialise(YouTubeVideo downloadVideo)
        {
            Task.Run(downloadVideo.LoadViews);
            DownloadVideo = downloadVideo;
        }

        public event EventHandler DownloadCompleted;
    }
}