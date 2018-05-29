namespace YouTube.Downloader.ViewModels.Interfaces
{
    using System.Collections.Generic;

    using Caliburn.Micro;

    using YouTube.Downloader.Models;

    internal interface IMatchedVideosViewModel : IViewModelBase
    {
        IObservableCollection<IMatchedVideoViewModel> Videos { get; }

        IObservableCollection<IMatchedVideoViewModel> SelectedVideos { get; }

        void Load(IEnumerable<QueryResult> videos);

        void DownloadSelected();
    }
}