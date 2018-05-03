namespace YouTube.Downloader.ViewModels
{
    using System.Windows;

    using Caliburn.Micro;

    using YouTube.Downloader.ViewModels.Interfaces;

    internal sealed class ShellViewModel : Conductor<IViewModelBase>, IShellViewModel
    {
        private readonly ISettingsViewModel _settingsViewModel;

        public ShellViewModel(IMainViewModel mainViewModel, ISettingsViewModel settingsViewModel)
        {
            DisplayName = "YouTube Downloader";

            _settingsViewModel = settingsViewModel;

            settingsViewModel.Closed += (sender, e) => ChangeActiveItem(mainViewModel, false);

            ActivateItem(mainViewModel);
        }

        public void Exit()
        {
            Application.Current.Shutdown();
        }

        public void OpenSettings()
        {
            ChangeActiveItem(_settingsViewModel, false);
        }
    }
}