namespace YouTube.Downloader.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows.Data;

    using Caliburn.Micro;

    using YouTube.Downloader.Core;
    using YouTube.Downloader.EventArgs;
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

        private bool _canPause;
        public bool CanPause
        {
            get => _canPause;

            set
            {
                if (_canPause == value) return;

                _canPause = value;
                NotifyOfPropertyChange(() => CanPause);
                NotifyOfPropertyChange(() => CanTogglePause);
            }
        }

        private bool _canResume;
        public bool CanResume
        {
            get => _canResume;

            set
            {
                if (_canResume == value) return;

                _canResume = value;
                NotifyOfPropertyChange(() => CanResume);
                NotifyOfPropertyChange(() => CanTogglePause);
            }
        }

        public bool CanTogglePause => CanPause || CanResume;

        public void TogglePause()
        {
            if (CanPause)
            {
                SelectedDownloads.Apply(downloadViewModel => downloadViewModel.Download.Pause());
                return;
            }

            if (CanResume)
            {
                _downloadService.ResumeDownloads(SelectedDownloads.Select(downloadViewModel => downloadViewModel.Download));
            }
        }

        public bool CanKill => SelectedDownloads.Any(downloadViewModel => downloadViewModel.Download.CanKill);

        public void Kill()
        {
            SelectedDownloads.Select(downloadViewModel => downloadViewModel.Download).Where(download => download.CanKill).Apply(download => download.Kill());
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
                void DownloadCompleted(object sender, DownloadFinishedEventArgs e)
                {
                    downloadViewModel.Download.Finished -= DownloadCompleted;
                    downloadViewModel.DownloadStatus.PropertyChanged -= DownloadStatusPropertyChanged;

                    DelayedCallbackHelper.SetTimeout(3_000, () =>
                    {
                        SelectedDownloads.Remove(downloadViewModel);
                        Downloads.Remove(downloadViewModel);
                    });
                }

                downloadViewModel.Download.Finished += DownloadCompleted;

                void DownloadStatusPropertyChanged(object sender, PropertyChangedEventArgs e)
                {
                    if (e.PropertyName != nameof(DownloadStatus.DownloadState)) return;

                    Downloads.Refresh();
                    RecomputeActionGuards();
                }

                downloadViewModel.DownloadStatus.PropertyChanged += DownloadStatusPropertyChanged;
            }

            Downloads.AddRange(downloads);
            _downloadService.QueueDownloads(downloads.Select(downloadViewModel => downloadViewModel.Download));
        }

        private void RecomputeActionGuards()
        {
            CanPause = SelectedDownloads.All(downloadViewModel => downloadViewModel.Download.CanPause);
            CanResume = !CanPause && SelectedDownloads.All(downloadViewModel => downloadViewModel.Download.CanResume);

            NotifyOfPropertyChange(() => CanKill);
        }
    }
}