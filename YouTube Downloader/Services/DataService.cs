namespace YouTube.Downloader.Services
{
    using System.Collections.Generic;
    using System.IO;

    using Newtonsoft.Json;

    using YouTube.Downloader.Services.Interfaces;

    internal class DataService : IDataService
    {
        private readonly IAppDataService _appDataService;

        public DataService(IAppDataService appDataService)
        {
            _appDataService = appDataService;

            _appDataService.GetFolder("Data");
        }

        public T[] Load<T>(string dataName)
        {
            return JsonConvert.DeserializeObject<T[]>(File.ReadAllText(_appDataService.GetFile($"Data/{dataName}.json")));
        }

        public void Save<T>(IEnumerable<T> data, string dataName)
        {
            File.WriteAllText(_appDataService.GetFile($"Data/{dataName}.json"), JsonConvert.SerializeObject(data));
        }
    }
}