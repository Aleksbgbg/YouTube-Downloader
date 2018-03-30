namespace YouTube.Downloader.ViewModels
{
    using YouTube.Downloader.Models;
    using YouTube.Downloader.Services.Interfaces;
    using YouTube.Downloader.ViewModels.Interfaces;

    internal class SettingsViewModel : ViewModelBase, ISettingsViewModel
    {
        public SettingsViewModel(ISettingsService settingsService)
        {
            Settings = settingsService.Settings;
        }

        public Settings Settings { get; }
    }
}