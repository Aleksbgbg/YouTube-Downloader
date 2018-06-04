namespace YouTube.Downloader.ViewModels.Process
{
    using YouTube.Downloader.Core.Downloading;
    using YouTube.Downloader.Models.Download;
    using YouTube.Downloader.ViewModels.Interfaces;
    using YouTube.Downloader.ViewModels.Interfaces.Process;

    internal class DownloadProcessViewModel : ProcessViewModel, IDownloadProcessViewModel
    {
        public DownloadProgress DownloadProgress { get; private set; }

        public void Initialise(IVideoViewModel videoViewModel, DownloadProcess process, DownloadProgress downloadProgress)
        {
            Initialise(videoViewModel, process);
            DownloadProgress = downloadProgress;
        }
    }
}