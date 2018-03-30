namespace YouTube.Downloader.ViewModels
{
    using Caliburn.Micro;

    using YouTube.Downloader.ViewModels.Interfaces;

    internal sealed class ShellViewModel : Conductor<IViewModelBase>, IShellViewModel
    {
        private readonly IMainViewModel _mainViewModel;

        public ShellViewModel(IMainViewModel mainViewModel)
        {
            DisplayName = "YouTube Downloader";

            _mainViewModel = mainViewModel;

            ActivateItem(mainViewModel);
        }

        public void ToggleSettings()
        {
            if (ActiveItem == _mainViewModel)
            {
                ChangeActiveItem(IoC.Get<ISettingsViewModel>(), false);
                return;
            }

            ChangeActiveItem(_mainViewModel, true);
        }
    }
}