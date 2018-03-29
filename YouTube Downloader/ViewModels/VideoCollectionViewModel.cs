namespace YouTube.Downloader.ViewModels
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows.Input;

    using Caliburn.Micro;

    using Fidl.Helpers;

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

        public ICommand SelectAllCommand => new RelayCommand<object>(_ =>
        {
            foreach (IYouTubeVideoViewModel video in Videos)
            {
                video.IsSelected = true;
            }
        });

        private int _selectedVideos;
        public int SelectedVideos
        {
            get => _selectedVideos;

            private set
            {
                if (_selectedVideos == value) return;

                _selectedVideos = value;
                NotifyOfPropertyChange(() => SelectedVideos);
            }
        }

        public void Load(IEnumerable<YouTubeVideo> videos)
        {
            void VideoPropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName != nameof(IYouTubeVideoViewModel.IsSelected)) return;

                if (((IYouTubeVideoViewModel)sender).IsSelected)
                {
                    SelectedVideos += 1;
                }
                else
                {
                    SelectedVideos -= 1;
                }
            }

            Videos.Apply(video => video.PropertyChanged -= VideoPropertyChanged);
            Videos.Clear();

            Videos.AddRange(videos.Select(_youTubeFactory.MakeVideoViewModel));
            Videos.Apply(video => video.PropertyChanged += VideoPropertyChanged);
        }
    }
}