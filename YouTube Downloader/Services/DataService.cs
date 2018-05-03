namespace YouTube.Downloader.Services
{
    using System.Collections.Generic;
    using System.IO;

    using Newtonsoft.Json;

    using YouTube.Downloader.Services.Interfaces;

    internal class DataService : IDataService
    {
        private const string EmptyData = "[]";

        private readonly IAppDataService _appDataService;

        public DataService(IAppDataService appDataService)
        {
            _appDataService = appDataService;

            _appDataService.GetFolder("Data");
        }

        public T[] Load<T>(string dataName)
        {
            string dataFile = _appDataService.GetFile($"Data/{dataName}.json", EmptyData);

            string fileData = File.ReadAllText(dataFile);

            if (fileData != EmptyData)
            {
                File.WriteAllText(dataFile, EmptyData); // Prevent duplicate loading of file if not cleared correctly
            }

            return JsonConvert.DeserializeObject<T[]>(fileData);
        }

        public void Save<T>(IEnumerable<T> data, string dataName)
        {
            File.WriteAllText(_appDataService.GetFile($"Data/{dataName}.json"), JsonConvert.SerializeObject(data));
        }
    }
}