namespace YouTube.Downloader.ViewModels
{
    using YouTube.Downloader.Core.Downloading;
    using YouTube.Downloader.Models.Download;
    using YouTube.Downloader.ViewModels.Interfaces;

    internal class DownloadViewModel : ViewModelBase, IDownloadViewModel
    {
        public IVideoViewModel VideoViewModel { get; private set; }

        public Download Download { get; private set; }

        public DownloadStatus DownloadStatus { get; } = new DownloadStatus();

        public void Initialise(IVideoViewModel videoViewModel, Download download)
        {
            VideoViewModel = videoViewModel;
            Download = download;
        }
    }
}