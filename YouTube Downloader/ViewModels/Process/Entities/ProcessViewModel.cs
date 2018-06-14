namespace YouTube.Downloader.ViewModels.Process.Entities
{
    using YouTube.Downloader.Models.Download;
    using YouTube.Downloader.ViewModels.Interfaces;
    using YouTube.Downloader.ViewModels.Interfaces.Process.Entities;

    internal abstract class ProcessViewModel : ViewModelBase, IProcessViewModel
    {
        public IVideoViewModel VideoViewModel { get; private set; }

        private DownloadState _downloadState;
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

        private protected void Initialise(IVideoViewModel videoViewModel)
        {
            VideoViewModel = videoViewModel;
        }
    }
}