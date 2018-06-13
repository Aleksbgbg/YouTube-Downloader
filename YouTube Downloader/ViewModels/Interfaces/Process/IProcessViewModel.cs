namespace YouTube.Downloader.ViewModels.Interfaces.Process
{
    using YouTube.Downloader.Models.Download;

    internal interface IProcessViewModel : IViewModelBase
    {
        IVideoViewModel VideoViewModel { get; }

        DownloadState DownloadState { get; set; }
    }
}