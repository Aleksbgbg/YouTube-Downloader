namespace YouTube.Downloader.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows.Data;

    using Caliburn.Micro;

    using YouTube.Downloader.Core;
    using YouTube.Downloader.Factories.Interfaces;
    using YouTube.Downloader.Models.Download;
    using YouTube.Downloader.Services.Interfaces;
    using YouTube.Downloader.ViewModels.Interfaces;

    internal class CurrentDownloadsViewModel : ViewModelBase, ICurrentDownloadsViewModel, IHandle<IEnumerable<IVideoViewModel>>
    {
        private readonly IDownloadService _downloadService;

        private readonly IDownloadFactory _downloadFactory;

        public CurrentDownloadsViewModel(IEventAggregator eventAggregator, IDownloadFactory downloadFactory, IVideoFactory videoFactory, IDataService dataService, IDownloadService downloadService)
        {
            eventAggregator.Subscribe(this);
            _downloadService = downloadService;
            _downloadFactory = downloadFactory;

            ((ListCollectionView)CollectionViewSource.GetDefaultView(Downloads)).CustomSort = Comparer<IDownloadViewModel>.Create((first, second) => -first.DownloadStatus.DownloadState.CompareTo(second.DownloadStatus.DownloadState));

            SelectedDownloads.CollectionChanged += (sender, e) => RecomputeActionGuards();
        }

        public IObservableCollection<IDownloadViewModel> Downloads { get; } = new BindableCollection<IDownloadViewModel>();

        public IObservableCollection<IDownloadViewModel> SelectedDownloads { get; } = new BindableCollection<IDownloadViewModel>();

        public bool CanKill => SelectedDownloads.Any(downloadViewModel => downloadViewModel.DownloadProcess.CanKill);

        public void Kill()
        {
            SelectedDownloads.Select(downloadViewModel => downloadViewModel.DownloadProcess).Where(download => download.CanKill).Apply(download => download.Kill());
        }

        public void Handle(IEnumerable<IVideoViewModel> message)
        {
            AddDownloads(message);
        }

        public void AddDownloads(IEnumerable<IVideoViewModel> videos)
        {
            IDownloadViewModel[] downloads = videos.Select(_downloadFactory.MakeDownloadViewModel).ToArray();

            foreach (IDownloadViewModel downloadViewModel in downloads)
            {
                void DownloadCompleted(object sender, EventArgs e)
                {
                    downloadViewModel.DownloadProcess.Exited -= DownloadCompleted;
                    downloadViewModel.DownloadStatus.PropertyChanged -= DownloadStatusPropertyChanged;

                    DelayedCallbackHelper.SetTimeout(3_000, () =>
                    {
                        SelectedDownloads.Remove(downloadViewModel);
                        Downloads.Remove(downloadViewModel);
                    });
                }

                downloadViewModel.DownloadProcess.Exited += DownloadCompleted;

                void DownloadStatusPropertyChanged(object sender, PropertyChangedEventArgs e)
                {
                    if (e.PropertyName != nameof(DownloadStatus.DownloadState)) return;

                    Downloads.Refresh();
                    RecomputeActionGuards();
                }

                downloadViewModel.DownloadStatus.PropertyChanged += DownloadStatusPropertyChanged;
            }

            Downloads.AddRange(downloads);
            _downloadService.QueueDownloads(downloads.Select(downloadViewModel => downloadViewModel.DownloadProcess));
        }

        private void RecomputeActionGuards()
        {
            NotifyOfPropertyChange(() => CanKill);
        }
    }
}