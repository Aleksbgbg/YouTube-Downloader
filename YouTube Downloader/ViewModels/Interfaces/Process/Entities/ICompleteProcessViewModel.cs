namespace YouTube.Downloader.ViewModels.Interfaces.Process.Entities
{
    using YouTube.Downloader.Models.Download;

    internal interface ICompleteProcessViewModel : IProcessViewModel
    {
        void Initialise(IVideoViewModel videoViewModel, DownloadState downloadState);
    }
}