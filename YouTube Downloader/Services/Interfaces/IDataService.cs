namespace YouTube.Downloader.Services.Interfaces
{
    using System.Collections.Generic;

    internal interface IDataService
    {
        void Save<T>(IEnumerable<T> data, string dataName);

        T[] Load<T>(string dataName);
    }
}