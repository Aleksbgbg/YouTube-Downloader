namespace YouTube.Downloader.ViewModels.Interfaces
{
    using System;

    using YouTube.Downloader.Models;

    internal interface IDownloadViewModel : IViewModelBase
    {
        IYouTubeVideoViewModel YouTubeVideoViewModel { get; }

        DownloadState DownloadState { get; set; }

        void Initialise(IYouTubeVideoViewModel youTubeVideoViewModel);

        event EventHandler DownloadCompleted;
    }
}