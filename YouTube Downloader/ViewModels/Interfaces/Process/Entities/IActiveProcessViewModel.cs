namespace YouTube.Downloader.ViewModels.Interfaces.Process.Entities
{
    using YouTube.Downloader.Utilities.Processing;

    internal interface IActiveProcessViewModel : IProcessViewModel
    {
        MonitoredProcess Process { get; }
    }
}