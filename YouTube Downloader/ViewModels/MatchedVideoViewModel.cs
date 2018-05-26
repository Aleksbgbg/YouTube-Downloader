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

        private bool _isExchanging;
        public bool IsExchanging
        {
            get => _isExchanging;

            private set
            {
                if (_isExchanging == value) return;

                _isExchanging = value;
                NotifyOfPropertyChange(() => IsExchanging);
            }
        }

        public IVideoViewModel VideoViewModel { get; private set; }

        public void Initialise(IVideoViewModel videoViewModel)
        {
            VideoViewModel = videoViewModel;
        }

        public void Exchange()
        {
            IsExchanging = true;

            IRequeryViewModel requeryViewModel = _requeryFactory.MakeRequeryViewModel(VideoViewModel);

            void RequeryViewModelDeactivated(object sender, Caliburn.Micro.DeactivationEventArgs e)
            {
                requeryViewModel.Deactivated -= RequeryViewModelDeactivated;
                IsExchanging = false;
            }

            requeryViewModel.Deactivated += RequeryViewModelDeactivated;
            _dialogService.ShowDialog(requeryViewModel);
        }
    }
}