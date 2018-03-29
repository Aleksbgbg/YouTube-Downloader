namespace YouTube.Downloader.ViewModels
{
    using Caliburn.Micro;

    using YouTube.Downloader.Factories.Interfaces;
    using YouTube.Downloader.Models;
    using YouTube.Downloader.ViewModels.Interfaces;

    internal class CurrentDownloadsViewModel : ViewModelBase, ICurrentDownloadsViewModel, IHandle<YouTubeVideo>
    {
        private readonly IDownloadFactory _downloadFactory;

        public CurrentDownloadsViewModel(IDownloadFactory downloadFactory)
        {
            _downloadFactory = downloadFactory;
        }

        public IObservableCollection<IDownloadViewModel> Downloads { get; } = new BindableCollection<IDownloadViewModel>();

        public void Handle(YouTubeVideo message)
        {
            IDownloadViewModel downloadViewModel = _downloadFactory.MakeDownloadViewModel(message);

            downloadViewModel.DownloadCompleted += (sender, e) => Downloads.Remove(downloadViewModel);

            Downloads.Add(downloadViewModel);
        }
    }
}