namespace YouTube.Downloader.ViewModels.Process
{
    using YouTube.Downloader.Core.Downloading;
    using YouTube.Downloader.ViewModels.Interfaces;
    using YouTube.Downloader.ViewModels.Interfaces.Process;

    internal abstract class ActiveProcessViewModel : ProcessViewModel, IActiveProcessViewModel
    {
        public MonitoredProcess Process { get; private set; }

        private protected void Initialise(IVideoViewModel videoViewModel, MonitoredProcess process)
        {
            Initialise(videoViewModel);
            Process = process;
        }
    }
}