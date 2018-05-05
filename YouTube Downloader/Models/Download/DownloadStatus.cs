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

        public DownloadProgress DownloadProgress { get; } = new DownloadProgress();
    }
}