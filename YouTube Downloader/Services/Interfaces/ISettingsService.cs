namespace YouTube.Downloader.Services.Interfaces
{
    using YouTube.Downloader.Models;

    internal interface ISettingsService
    {
        Settings Settings { get; }

        void Save();
    }
}