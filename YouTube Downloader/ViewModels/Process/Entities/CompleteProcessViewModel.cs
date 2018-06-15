namespace YouTube.Downloader.ViewModels.Process.Entities
{
    using YouTube.Downloader.Models.Download;
    using YouTube.Downloader.ViewModels.Interfaces;
    using YouTube.Downloader.ViewModels.Interfaces.Process.Entities;

    internal class CompleteProcessViewModel : ProcessViewModel, ICompleteProcessViewModel
    {
        public void Initialise(IVideoViewModel videoViewModel, DownloadState downloadState)
        {
            Initialise(videoViewModel);
            DownloadState = downloadState;
        }
    }
}