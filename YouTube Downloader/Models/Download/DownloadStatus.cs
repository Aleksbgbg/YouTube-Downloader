namespace YouTube.Downloader.Models.Download
{
    using Caliburn.Micro;

    internal class DownloadStatus : PropertyChangedBase
    {
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

        private DownloadProgress _downloadProgress = new DownloadProgress();
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
    }
}