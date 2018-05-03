namespace YouTube.Downloader.ViewModels
{
    using YouTube.Downloader.ViewModels.Interfaces;

    internal class MatchedVideoViewModel : ViewModelBase, IMatchedVideoViewModel
    {
        public IVideoViewModel VideoViewModel { get; private set; }

        public void Initialise(IVideoViewModel videoViewModel)
        {
            VideoViewModel = videoViewModel;
        }
    }
}