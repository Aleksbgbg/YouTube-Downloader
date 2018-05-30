namespace YouTube.Downloader.ViewModels.Process
{
    using YouTube.Downloader.Core.Downloading;
    using YouTube.Downloader.Models.Download;
    using YouTube.Downloader.ViewModels.Interfaces;
    using YouTube.Downloader.ViewModels.Interfaces.Process;

    internal class ProcessViewModel : ViewModelBase, IProcessViewModel
    {
        public IVideoViewModel VideoViewModel { get; private set; }

        public MonitoredProcess Process { get; private set; }

        public DownloadStatus DownloadStatus { get; } = new DownloadStatus();

        public void Initialise(IVideoViewModel videoViewModel, MonitoredProcess process)
        {
            VideoViewModel = videoViewModel;
            Process = process;
        }
    }
}