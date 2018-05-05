namespace YouTube.Downloader.ViewModels
{
    using YouTube.Downloader.Core.Downloading;
    using YouTube.Downloader.Models;
    using YouTube.Downloader.ViewModels.Interfaces;

    internal class DownloadViewModel : ViewModelBase, IDownloadViewModel
    {
        public IVideoViewModel VideoViewModel { get; private set; }

        public Download Download { get; private set; }

        private DownloadState _downloadState = DownloadState.Queued;
        public DownloadState DownloadState
        {
            get => _downloadState;

            set
            {
                if (_downloadState == value) return;

                _downloadState = value;
                NotifyOfPropertyChange(() => DownloadState);
            }
        }

        private DownloadProgress _downloadProgress;
        public DownloadProgress DownloadProgress
        {
            get => _downloadProgress;

            set
            {
                if (_downloadProgress == value) return;

                _downloadProgress = value;
                NotifyOfPropertyChange(() => DownloadProgress);
            }
        }

        public void Initialise(IVideoViewModel videoViewModel, Download download)
        {
            VideoViewModel = videoViewModel;
            Download = download;
        }
    }
}