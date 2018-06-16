namespace YouTube.Downloader.ViewModels.Interfaces.Process.Entities
{
    using YouTube.Downloader.Utilities.Downloading;

    internal interface IActiveProcessViewModel : IProcessViewModel
    {
        MonitoredProcess Process { get; }
    }
}