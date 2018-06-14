namespace YouTube.Downloader.ViewModels.Interfaces.Process.Entities
{
    internal interface ICompleteProcessViewModel : IProcessViewModel
    {
        void Initialise(IVideoViewModel videoViewModel);
    }
}