namespace YouTube.Downloader.ViewModels
{
    using YouTube.Downloader.Factories.Interfaces;
    using YouTube.Downloader.Services.Interfaces;
    using YouTube.Downloader.ViewModels.Interfaces;

    internal class MatchedVideoViewModel : ViewModelBase, IMatchedVideoViewModel
    {
        private readonly IRequeryFactory _requeryFactory;

        private readonly IDialogService _dialogService;

        public MatchedVideoViewModel(IRequeryFactory requeryFactory, IDialogService dialogService)
        {
            _requeryFactory = requeryFactory;
            _dialogService = dialogService;
        }

        public IVideoViewModel VideoViewModel { get; private set; }

        public void Initialise(IVideoViewModel videoViewModel)
        {
            VideoViewModel = videoViewModel;
        }

        public void Exchange()
        {
            _dialogService.ShowDialog(_requeryFactory.MakeRequeryViewModel(VideoViewModel));
        }
    }
}