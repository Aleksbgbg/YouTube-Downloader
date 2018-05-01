namespace YouTube.Downloader.ViewModels
{
    using System;

    using YouTube.Downloader.Core.Downloading;
    using YouTube.Downloader.Models;
    using YouTube.Downloader.ViewModels.Interfaces;

    internal class DownloadViewModel : ViewModelBase, IDownloadViewModel
    {
        public event EventHandler DownloadCompleted;

        public IVideoViewModel VideoViewModel { get; private set; }

        private DownloadState _downloadState = DownloadState.Queued;
        public DownloadState DownloadState
        {
            get => _downloadState;

            set
            {
                if (_downloadState == value) return;

                _downloadState = value;
                NotifyOfPropertyChange(() => DownloadState);

                if (_downloadState == DownloadState.Completed || _downloadState == DownloadState.Exited)
                {
                    DownloadCompleted?.Invoke(this, EventArgs.Empty);
                }
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

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;

            set
            {
                if (_isSelected == value) return;

                _isSelected = value;
                NotifyOfPropertyChange(() => IsSelected);
            }
        }

        public Download Download { get; set; }

        public void Initialise(IVideoViewModel videoViewModel)
        {
            VideoViewModel = videoViewModel;
        }
    }
}