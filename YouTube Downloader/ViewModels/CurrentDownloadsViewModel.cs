namespace YouTube.Downloader.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Caliburn.Micro;

    using YouTube.Downloader.Factories.Interfaces;
    using YouTube.Downloader.Models;
    using YouTube.Downloader.ViewModels.Interfaces;

    internal class CurrentDownloadsViewModel : ViewModelBase, ICurrentDownloadsViewModel, IHandle<IEnumerable<YouTubeVideo>>
    {
        private readonly IDownloadFactory _downloadFactory;

        public CurrentDownloadsViewModel(IEventAggregator eventAggregator, IDownloadFactory downloadFactory)
        {
            eventAggregator.Subscribe(this);
            _downloadFactory = downloadFactory;
        }

        public IObservableCollection<IDownloadViewModel> Downloads { get; } = new BindableCollection<IDownloadViewModel>();

        public void Handle(IEnumerable<YouTubeVideo> message)
        {
            foreach (IDownloadViewModel downloadViewModel in message.Select(_downloadFactory.MakeDownloadViewModel))
            {
                void DownloadViewModelDownloadCompleted(object sender, EventArgs e)
                {
                    downloadViewModel.DownloadCompleted -= DownloadViewModelDownloadCompleted;
                    Downloads.Remove(downloadViewModel);
                }

                downloadViewModel.DownloadCompleted += DownloadViewModelDownloadCompleted;
                Downloads.Add(downloadViewModel);
            }
        }
    }
}