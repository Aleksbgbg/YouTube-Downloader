namespace YouTube.Downloader.ViewModels.Interfaces
{
    using System.Collections.Generic;

    using Caliburn.Micro;

    internal interface ICurrentDownloadsViewModel : IViewModelBase
    {
        IObservableCollection<IDownloadViewModel> Downloads { get; }

        IObservableCollection<IDownloadViewModel> SelectedDownloads { get; }

        void AddDownloads(IEnumerable<IVideoViewModel> videos);
    }
}