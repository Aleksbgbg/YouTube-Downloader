namespace YouTube.Downloader.Services.Interfaces
{
    internal interface IAppDataService
    {
        string GetFolder(string name);

        string GetFile(string name);
    }
}