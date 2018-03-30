namespace YouTube.Downloader.ViewModels.Interfaces
{
    using System;

    using YouTube.Downloader.Models;

    internal interface IDownloadViewModel : IViewModelBase
    {
        YouTubeVideo DownloadVideo { get; }

        DownloadState DownloadState { get; set; }

        void Initialise(YouTubeVideo downloadVideo);

        event EventHandler DownloadCompleted;
    }
}