namespace YouTube.Downloader.Services
{
    using System.IO;

    using Newtonsoft.Json;

    using YouTube.Downloader.Services.Interfaces;

    internal class DataService : IDataService
    {
        private readonly IAppDataService _appDataService;

        public DataService(IAppDataService appDataService)
        {
            _appDataService = appDataService;
        }

        public T Load<T>(string dataName, string emptyData = "")
        {
            return JsonConvert.DeserializeObject<T>(File.ReadAllText(_appDataService.GetFile($"Data/{dataName}.json", emptyData)));
        }

        public T LoadAndWipe<T>(string dataName, string emptyData = "")
        {
            string dataFile = _appDataService.GetFile($"Data/{dataName}.json", emptyData);

            string fileData = File.ReadAllText(dataFile);

            if (fileData != emptyData)
            {
                File.WriteAllText(dataFile, emptyData); // Prevent duplicate loading of file if not cleared correctly
            }

            return JsonConvert.DeserializeObject<T>(fileData);
        }

        public void Save<T>(T data, string dataName)
        {
            File.WriteAllText(_appDataService.GetFile($"Data/{dataName}.json"), JsonConvert.SerializeObject(data));
        }
    }
}