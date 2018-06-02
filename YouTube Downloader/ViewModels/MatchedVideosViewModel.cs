namespace YouTube.Downloader.ViewModels
{
    using System.Collections.Generic;
    using System.Linq;

    using Caliburn.Micro;

    using YouTube.Downloader.Core;
    using YouTube.Downloader.Factories.Interfaces;
    using YouTube.Downloader.Models;
    using YouTube.Downloader.ViewModels.Interfaces;

    internal class MatchedVideosViewModel : ViewModelBase, IMatchedVideosViewModel
    {
        private readonly IEventAggregator _eventAggregator;

        private readonly IProcessFactory _processFactory;

        private readonly IVideoFactory _videoFactory;

        public MatchedVideosViewModel(IEventAggregator eventAggregator, IProcessFactory processFactory, IVideoFactory videoFactory)
        {
            _eventAggregator = eventAggregator;
            _processFactory = processFactory;
            _videoFactory = videoFactory;
        }

        public IObservableCollection<IMatchedVideoViewModel> Videos { get; } = new BindableCollection<IMatchedVideoViewModel>();

        public IObservableCollection<IMatchedVideoViewModel> SelectedVideos { get; } = new BindableCollection<IMatchedVideoViewModel>();

        public void Load(IEnumerable<YouTubeVideo> videos)
        {
            SelectedVideos.Clear();
            Videos.Clear();

            Videos.AddRange(videos.Select(_videoFactory.MakeMatchedVideoViewModel));
        }

        public void DownloadSelected()
        {
            _eventAggregator.BeginPublishOnUIThread(new ProcessTransferMessage(ProcessTransferType.Download,
                                                                               SelectedVideos.Select(matchedVideo => matchedVideo.VideoViewModel)
                                                                                             .Select(_processFactory.MakeDownloadProcessViewModel)));
        }
    }
}