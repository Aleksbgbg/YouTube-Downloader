namespace YouTube.Downloader.ViewModels
{
    using System.Threading.Tasks;
    using System.Windows;

    using Caliburn.Micro;

    using YouTube.Downloader.Utilities;
    using YouTube.Downloader.ViewModels.Interfaces;

    internal sealed class ShellViewModel : Conductor<IViewModelBase>, IShellViewModel
    {
        private readonly IMainViewModel _mainViewModel;

        private readonly ISettingsViewModel _settingsViewModel;

        public ShellViewModel(IMainViewModel mainViewModel, ISettingsViewModel settingsViewModel)
        {
            DisplayName = "YouTube Downloader";

            _mainViewModel = mainViewModel;
            _settingsViewModel = settingsViewModel;

            settingsViewModel.Closed += (sender, e) => ChangeActiveItem(mainViewModel, false);

            Task.Run(Initialize);
        }

        public void Exit()
        {
            Application.Current.Shutdown();
        }

        public void OpenSettings()
        {
            ChangeActiveItem(_settingsViewModel, false);
        }

        private async Task Initialize()
        {
            await YouTubeDl.Update();

            ActivateItem(_mainViewModel);
        }
    }
}