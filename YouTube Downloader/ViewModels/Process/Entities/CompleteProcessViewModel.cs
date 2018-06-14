namespace YouTube.Downloader.ViewModels.Process.Entities
{
    using YouTube.Downloader.Models.Download;
    using YouTube.Downloader.ViewModels.Interfaces;
    using YouTube.Downloader.ViewModels.Interfaces.Process.Entities;

    internal class CompleteProcessViewModel : ProcessViewModel, ICompleteProcessViewModel
    {
        public new void Initialise(IVideoViewModel videoViewModel)
        {
            base.Initialise(videoViewModel);
            DownloadState = DownloadState.Completed;
        }
    }
}