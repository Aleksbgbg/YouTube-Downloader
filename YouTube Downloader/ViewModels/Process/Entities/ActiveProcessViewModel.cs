namespace YouTube.Downloader.ViewModels.Process.Entities
{
    using YouTube.Downloader.Utilities.Processing;
    using YouTube.Downloader.ViewModels.Interfaces;
    using YouTube.Downloader.ViewModels.Interfaces.Process.Entities;

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