namespace YouTube.Downloader.ViewModels
{
    using System.Collections.Generic;
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

        public void Load(IEnumerable<YouTubeVideo> videos)
        {
            Videos.Clear();
            Videos.AddRange(videos.Select(_youTubeFactory.MakeVideoViewModel));
        }
    }
}