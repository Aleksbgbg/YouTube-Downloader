namespace YouTube.Downloader.ViewModels.Process.Entities
{
    using YouTube.Downloader.Models.Download;
    using YouTube.Downloader.Utilities.Processing;
    using YouTube.Downloader.ViewModels.Interfaces;
    using YouTube.Downloader.ViewModels.Interfaces.Process.Entities;

    internal class DownloadProcessViewModel : ActiveProcessViewModel, IDownloadProcessViewModel
    {
        public DownloadProgress DownloadProgress { get; private set; }

        public void Initialise(IVideoViewModel videoViewModel, DownloadProcess process, DownloadProgress downloadProgress)
        {
            Initialise(videoViewModel, process);
            DownloadProgress = downloadProgress;
        }
    }
}