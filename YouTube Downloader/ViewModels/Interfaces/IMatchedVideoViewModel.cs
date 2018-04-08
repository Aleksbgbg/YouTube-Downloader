namespace YouTube.Downloader.ViewModels.Interfaces
{
    internal interface IMatchedVideoViewModel : IViewModelBase
    {
        bool IsSelected { get; set; }

        IVideoViewModel VideoViewModel { get; }

        void Initialise(IVideoViewModel videoViewModel);
    }
}