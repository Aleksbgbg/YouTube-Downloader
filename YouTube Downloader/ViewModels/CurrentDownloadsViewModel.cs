namespace YouTube.Downloader.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Caliburn.Micro;

    using YouTube.Downloader.Factories.Interfaces;
    using YouTube.Downloader.Services.Interfaces;
    using YouTube.Downloader.ViewModels.Interfaces;

    internal class CurrentDownloadsViewModel : ViewModelBase, ICurrentDownloadsViewModel, IHandle<IEnumerable<IVideoViewModel>>
    {
        private readonly IDownloadService _downloadService;

        private readonly IDownloadFactory _downloadFactory;

        public CurrentDownloadsViewModel(IEventAggregator eventAggregator, IDownloadService downloadService, IDownloadFactory downloadFactory)
        {
            eventAggregator.Subscribe(this);
            _downloadService = downloadService;
            _downloadFactory = downloadFactory;
        }

        public IObservableCollection<IDownloadViewModel> Downloads { get; } = new BindableCollection<IDownloadViewModel>();

        public void Handle(IEnumerable<IVideoViewModel> message)
        {
            IDownloadViewModel[] newDownloads = message.Select(downloadVideo => _downloadFactory.MakeDownloadViewModel(downloadVideo)).ToArray();

            foreach (IDownloadViewModel download in newDownloads)
            {
                void DownloadViewModelDownloadCompleted(object sender, EventArgs e)
                {
                    download.DownloadCompleted -= DownloadViewModelDownloadCompleted;
                    Downloads.Remove(download);
                }

                download.DownloadCompleted += DownloadViewModelDownloadCompleted;
            }

            Downloads.AddRange(newDownloads);

            _downloadService.QueueDownloads(newDownloads);
        }
    }
}