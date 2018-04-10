namespace YouTube.Downloader.ViewModels
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    using Caliburn.Micro;

    using YouTube.Downloader.Factories.Interfaces;
    using YouTube.Downloader.Models;
    using YouTube.Downloader.ViewModels.Interfaces;

    internal class MatchedVideosViewModel : ViewModelBase, IMatchedVideosViewModel
    {
        private readonly IEventAggregator _eventAggregator;

        private readonly IVideoFactory _videoFactory;

        public MatchedVideosViewModel(IEventAggregator eventAggregator, IVideoFactory videoFactory)
        {
            _eventAggregator = eventAggregator;
            _videoFactory = videoFactory;
        }

        public IObservableCollection<IMatchedVideoViewModel> Videos { get; } = new BindableCollection<IMatchedVideoViewModel>();

        public IObservableCollection<IMatchedVideoViewModel> SelectedVideos { get; } = new BindableCollection<IMatchedVideoViewModel>();

        public void Onload(IEnumerable<YouTubeVideo> videos)
        {
            IMatchedVideoViewModel[] newVideos = videos.Select(_videoFactory.MakeMatchedVideoViewModel).ToArray();

            Videos.AddRange(newVideos);

            newVideos.Apply(video => video.PropertyChanged += VideoPropertyChanged);
        }

        public void Clear()
        {
            Videos.Apply(video => video.PropertyChanged -= VideoPropertyChanged);
            SelectedVideos.Clear();
            Videos.Clear();
        }

        public void DownloadSelected()
        {
            _eventAggregator.BeginPublishOnUIThread(SelectedVideos.Select(matchedVideo => matchedVideo.VideoViewModel));
        }

        private void VideoPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(IMatchedVideoViewModel.IsSelected)) return;

            IMatchedVideoViewModel video = (IMatchedVideoViewModel)sender;

            if (video.IsSelected)
            {
                SelectedVideos.Add(video);
            }
            else
            {
                SelectedVideos.Remove(video);
            }
        }
    }
}