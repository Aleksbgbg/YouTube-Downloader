namespace YouTube.Downloader.ViewModels
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    using Caliburn.Micro;

    using YouTube.Downloader.Factories.Interfaces;
    using YouTube.Downloader.Models;
    using YouTube.Downloader.ViewModels.Interfaces;

    internal class VideoCollectionViewModel : ViewModelBase, IVideoCollectionViewModel
    {
        private readonly IEventAggregator _eventAggregator;

        private readonly IYouTubeFactory _youTubeFactory;

        public VideoCollectionViewModel(IEventAggregator eventAggregator, IYouTubeFactory youTubeFactory)
        {
            _eventAggregator = eventAggregator;
            _youTubeFactory = youTubeFactory;
        }

        public IObservableCollection<IYouTubeVideoViewModel> Videos { get; } = new BindableCollection<IYouTubeVideoViewModel>();

        public IObservableCollection<IYouTubeVideoViewModel> SelectedVideos { get; } = new BindableCollection<IYouTubeVideoViewModel>();

        public void Load(IEnumerable<YouTubeVideo> videos)
        {
            void VideoPropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName != nameof(IYouTubeVideoViewModel.IsSelected)) return;

                IYouTubeVideoViewModel video = (IYouTubeVideoViewModel)sender;

                if (video.IsSelected)
                {
                    SelectedVideos.Add(video);
                }
                else
                {
                    SelectedVideos.Remove(video);
                }
            }

            Videos.Apply(video => video.PropertyChanged -= VideoPropertyChanged);
            SelectedVideos.Clear();
            Videos.Clear();

            Videos.AddRange(videos.Select(_youTubeFactory.MakeVideoViewModel));
            Videos.Apply(video => video.PropertyChanged += VideoPropertyChanged);
        }

        public void DownloadSelected()
        {
            _eventAggregator.BeginPublishOnUIThread(SelectedVideos.Select(videoViewModel => videoViewModel.Video));
        }
    }
}