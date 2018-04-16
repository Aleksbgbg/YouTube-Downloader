namespace YouTube.Downloader.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Data;

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

            ((ListCollectionView)CollectionViewSource.GetDefaultView(Downloads)).CustomSort = Comparer<IDownloadViewModel>.Create((first, second) => -first.DownloadState.CompareTo(second.DownloadState));
        }

        public IObservableCollection<IDownloadViewModel> Downloads { get; } = new BindableCollection<IDownloadViewModel>();

        public void Handle(IEnumerable<IVideoViewModel> message)
        {
            IDownloadViewModel[] newDownloads = message.Select(_downloadFactory.MakeDownloadViewModel).ToArray();

            foreach (IDownloadViewModel download in newDownloads)
            {
                void DownloadViewModelDownloadCompleted(object sender, EventArgs e)
                {
                    download.DownloadCompleted -= DownloadViewModelDownloadCompleted;
                    download.PropertyChanged -= DownloadPropertyChanged;

                    Task.Delay(3_000).ContinueWith(task => Downloads.Remove(download));
                }

                download.DownloadCompleted += DownloadViewModelDownloadCompleted;
            }

            void DownloadPropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                Downloads.Refresh();
            }

            newDownloads.Apply(download => download.PropertyChanged += DownloadPropertyChanged);

            Downloads.AddRange(newDownloads);

            _downloadService.QueueDownloads(newDownloads);
        }
    }
}