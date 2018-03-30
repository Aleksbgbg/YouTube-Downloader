namespace YouTube.Downloader.ViewModels.Interfaces
{
    using YouTube.Downloader.Models;

    internal interface ISettingsViewModel : IViewModelBase
    {
        Settings Settings { get; }
    }
}