namespace YouTube.Downloader.Services
{
    using System;
    using System.IO;

    using YouTube.Downloader.Services.Interfaces;

    internal class AppDataService : IAppDataService
    {
        private static readonly string AppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        private static readonly string ApplicationPath = Path.Combine(AppDataPath, "YouTube Downloader");

        public AppDataService()
        {
            if (!Directory.Exists(ApplicationPath))
            {
                Directory.CreateDirectory(ApplicationPath);
            }
        }

        public string GetFolder(string name)
        {
            string directoryPath = Path.Combine(ApplicationPath, name);

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            return directoryPath;
        }

        public string GetFile(string name, string defaultContents = "")
        {
            string filePath = Path.Combine(ApplicationPath, name);

            if (!File.Exists(filePath))
            {
                string directory = Path.GetDirectoryName(filePath);

                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                File.WriteAllText(filePath, defaultContents);
            }

            return filePath;
        }
    }
}