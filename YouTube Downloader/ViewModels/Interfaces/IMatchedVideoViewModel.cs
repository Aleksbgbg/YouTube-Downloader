namespace YouTube.Downloader.ViewModels.Interfaces
{
    internal interface IMatchedVideoViewModel : IViewModelBase
    {
        IVideoViewModel VideoViewModel { get; }

        void Initialise(IVideoViewModel videoViewModel);
    }
}