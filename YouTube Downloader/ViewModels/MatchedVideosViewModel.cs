namespace YouTube.Downloader.ViewModels
{
    using System.Collections.Generic;
    using System.Linq;

    using Caliburn.Micro;

    using YouTube.Downloader.Factories.Interfaces;
    using YouTube.Downloader.Models;
    using YouTube.Downloader.Services.Interfaces;
    using YouTube.Downloader.ViewModels.Interfaces;

    internal class MatchedVideosViewModel : ViewModelBase, IMatchedVideosViewModel
    {
        private readonly IVideoFactory _videoFactory;

        private readonly IProcessDispatcherService _processDispatcherService;

        public MatchedVideosViewModel(IVideoFactory videoFactory, IProcessDispatcherService processDispatcherService)
        {
            _videoFactory = videoFactory;
            _processDispatcherService = processDispatcherService;
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
            _processDispatcherService.Dispatch(SelectedVideos.Select(matchedVideo => matchedVideo.VideoViewModel));
        }
    }
}