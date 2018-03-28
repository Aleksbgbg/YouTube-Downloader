namespace YouTube.Downloader.ViewModels.Interfaces
{
    using System.Collections.Generic;

    using Caliburn.Micro;

    using YouTube.Downloader.Models;

    internal interface IVideoCollectionViewModel : IViewModelBase
    {
        IObservableCollection<IYouTubeVideoViewModel> Videos { get; }

        void Load(IEnumerable<YouTubeVideo> videos);
    }
}