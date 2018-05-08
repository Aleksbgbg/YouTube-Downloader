namespace YouTube.Downloader.Services.Interfaces
{
    internal interface IDataService
    {
        T Load<T>(string dataName, string emptyData = "");

        T LoadAndWipe<T>(string dataName, string emptyData = "");

        void Save<T>(string dataName, T data);
    }
}