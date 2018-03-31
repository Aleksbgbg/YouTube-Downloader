namespace YouTube.Downloader.ViewModels
{
    using Caliburn.Micro;

    using YouTube.Downloader.ViewModels.Interfaces;

    internal sealed class ShellViewModel : Conductor<IViewModelBase>.Collection.OneActive, IShellViewModel
    {
        public ShellViewModel(IMainViewModel mainViewModel, ISettingsViewModel settingsViewModel)
        {
            DisplayName = "YouTube Downloader";

            Items.Add(mainViewModel);
            Items.Add(settingsViewModel);

            settingsViewModel.Closed += (sender, e) => ChangeActiveItem(mainViewModel, false);

            ActivateItem(mainViewModel);
        }

        public void OpenSettings()
        {
            ChangeActiveItem(Items[1], false);
        }
    }
}