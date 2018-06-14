namespace YouTube.Downloader.ViewModels.Interfaces.Process.Entities
{
    using YouTube.Downloader.Core.Downloading;

    internal interface IActiveProcessViewModel : IProcessViewModel
    {
        MonitoredProcess Process { get; }
    }
}