namespace YouTube.Downloader.ViewModels.Process
{
    using YouTube.Downloader.Core.Downloading;
    using YouTube.Downloader.Models.Download;
    using YouTube.Downloader.ViewModels.Interfaces;
    using YouTube.Downloader.ViewModels.Interfaces.Process;

    internal abstract class ProcessViewModel : ViewModelBase, IProcessViewModel
    {
        public IVideoViewModel VideoViewModel { get; private set; }

        public MonitoredProcess Process { get; private set; }

        public DownloadState DownloadState { get; set; }

        private protected void Initialise(IVideoViewModel videoViewModel, MonitoredProcess process)
        {
            VideoViewModel = videoViewModel;
            Process = process;
        }
    }
}