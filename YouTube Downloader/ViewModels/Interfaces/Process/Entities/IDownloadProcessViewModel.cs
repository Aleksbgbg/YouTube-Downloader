namespace YouTube.Downloader.ViewModels.Interfaces.Process.Entities
{
    using YouTube.Downloader.Models.Download;
    using YouTube.Downloader.Utilities.Downloading;

    internal interface IDownloadProcessViewModel : IActiveProcessViewModel
    {
        DownloadProgress DownloadProgress { get; }

        void Initialise(IVideoViewModel videoViewModel, DownloadProcess process, DownloadProgress downloadProgress);
    }
}