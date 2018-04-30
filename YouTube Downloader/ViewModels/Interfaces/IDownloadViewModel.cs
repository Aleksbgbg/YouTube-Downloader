namespace YouTube.Downloader.ViewModels.Interfaces
{
    using System;

    using YouTube.Downloader.Core.Downloading;
    using YouTube.Downloader.Models;

    internal interface IDownloadViewModel : IViewModelBase
    {
        IVideoViewModel VideoViewModel { get; }

        DownloadState DownloadState { get; set; }

        DownloadProgress DownloadProgress { get; set; }

        Download Download { get; set; }

        void Initialise(IVideoViewModel videoViewModel);

        event EventHandler DownloadCompleted;
    }
}