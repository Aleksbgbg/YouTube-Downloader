namespace YouTube.Downloader.Services
{
    using System.IO;

    using Newtonsoft.Json;

    using YouTube.Downloader.Models;
    using YouTube.Downloader.Models.Download;
    using YouTube.Downloader.Services.Interfaces;
    using YouTube.Downloader.Utilities.Interfaces;

    internal class SettingsService : ISettingsService
    {
        private readonly IDataService _dataService;

        public SettingsService(IDataService dataService, IFileSystemUtility fileSystemUtility)
        {
            _dataService = dataService;

            Settings = dataService.Load<Settings>("Settings", () => JsonConvert.SerializeObject(new Settings
            {
                    DownloadPath = Path.Combine(fileSystemUtility.DownloadsFolderPath, "YouTube Downloader"),
                    DownloadType = DownloadType.AudioVideo,
                    OutputFormat = OutputFormat.Auto
            }));
        }

        public Settings Settings { get; }

        public void Save()
        {
            _dataService.Save("Settings", Settings);
        }
    }
}