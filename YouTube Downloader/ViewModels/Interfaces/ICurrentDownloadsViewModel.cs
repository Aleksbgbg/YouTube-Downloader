namespace YouTube.Downloader.ViewModels.Interfaces
{
    using Caliburn.Micro;

    internal interface ICurrentDownloadsViewModel : IViewModelBase
    {
        IObservableCollection<IDownloadViewModel> Downloads { get; }

        IObservableCollection<IDownloadViewModel> SelectedDownloads { get; }
    }
}