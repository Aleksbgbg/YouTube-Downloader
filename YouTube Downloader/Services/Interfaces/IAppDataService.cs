namespace YouTube.Downloader.Services.Interfaces
{
    using System;

    internal interface IAppDataService
    {
        string GetFolder(string name);

        string GetFile(string name, string defaultContents = "");

        string GetFile(string name, Func<string> defaultContents);
    }
}