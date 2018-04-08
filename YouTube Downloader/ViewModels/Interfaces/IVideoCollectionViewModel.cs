namespace YouTube.Downloader.ViewModels.Interfaces
{
    using System.Collections.Generic;

    using Caliburn.Micro;

    using YouTube.Downloader.Models;

    internal interface IVideoCollectionViewModel : IViewModelBase
    {
        IObservableCollection<IMatchedVideoViewModel> Videos { get; }

        IObservableCollection<IMatchedVideoViewModel> SelectedVideos { get; }

        void Load(IEnumerable<YouTubeVideo> videos);

        void DownloadSelected();
    }
}