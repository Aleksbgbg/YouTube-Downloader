namespace YouTube.Downloader.Services
{
    using System.IO;

    using Newtonsoft.Json;

    using YouTube.Downloader.Models;
    using YouTube.Downloader.Services.Interfaces;

    internal class SettingsService : ISettingsService
    {
        private readonly string _settingsFile;

        public SettingsService(IAppDataService appDataService)
        {
            string settingsFolder = appDataService.GetFolder("Settings");

            _settingsFile = appDataService.GetFile(Path.Combine(settingsFolder, "Settings.json"));

            Settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(_settingsFile));
        }

        public Settings Settings { get; }
    }
}