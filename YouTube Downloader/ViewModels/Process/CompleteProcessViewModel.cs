namespace YouTube.Downloader.ViewModels.Process
{
    using YouTube.Downloader.Models.Download;
    using YouTube.Downloader.ViewModels.Interfaces;
    using YouTube.Downloader.ViewModels.Interfaces.Process;

    internal class CompleteProcessViewModel : ProcessViewModel, ICompleteProcessViewModel
    {
        public void Initialise(IVideoViewModel videoViewModel)
        {
            Initialise(videoViewModel, null);
            DownloadState = DownloadState.Completed;
        }
    }
}