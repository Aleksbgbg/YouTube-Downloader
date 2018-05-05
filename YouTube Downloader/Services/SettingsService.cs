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
        private readonly string _settingsFile;

        public SettingsService(IAppDataService appDataService, IFileSystemUtility fileSystemUtility)
        {
            string settingsFolder = appDataService.GetFolder("Settings");

            _settingsFile = appDataService.GetFile(Path.Combine(settingsFolder, "Settings.json"), JsonConvert.SerializeObject(new Settings
            {
                    DownloadPath = Path.Combine(fileSystemUtility.DownloadsFolderPath, "YouTube Downloader"),
                    DownloadType = DownloadType.AudioVideo
            }));

            Settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(_settingsFile));
        }

        public Settings Settings { get; }

        public void Save()
        {
            File.WriteAllText(_settingsFile, JsonConvert.SerializeObject(Settings));
        }
    }
}