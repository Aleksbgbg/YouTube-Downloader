namespace YouTube.Downloader.ViewModels.Interfaces.Process
{
    using YouTube.Downloader.Core.Downloading;

    internal interface IActiveProcessViewModel : IProcessViewModel
    {
        MonitoredProcess Process { get; }
    }
}