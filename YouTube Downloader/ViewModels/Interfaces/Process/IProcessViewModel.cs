namespace YouTube.Downloader.ViewModels.Interfaces.Process
{
    using YouTube.Downloader.Core.Downloading;
    using YouTube.Downloader.Models.Download;

    internal interface IProcessViewModel : IViewModelBase
    {
        IVideoViewModel VideoViewModel { get; }

        MonitoredProcess Process { get; }

        DownloadState DownloadState { get; set; }
    }
}