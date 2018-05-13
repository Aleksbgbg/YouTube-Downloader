namespace YouTube.Downloader.ViewModels
{
    using System.Collections.Generic;
    using System.Linq;

    using Caliburn.Micro;

    using YouTube.Downloader.Factories.Interfaces;
    using YouTube.Downloader.Models;
    using YouTube.Downloader.Services.Interfaces;
    using YouTube.Downloader.ViewModels.Interfaces;

    internal class RequeryViewModel : ViewModelBase, IRequeryViewModel
    {
        private readonly IVideoFactory _videoFactory;

        private readonly IYouTubeApiService _youTubeApiService;

        public RequeryViewModel(IVideoFactory videoFactory, IYouTubeApiService youTubeApiService)
        {
            _videoFactory = videoFactory;
            _youTubeApiService = youTubeApiService;
        }

        public IObservableCollection<IMatchedVideoViewModel> Results { get; } = new BindableCollection<IMatchedVideoViewModel>();

        public IEnumerable<IResult> Search(string query)
        {
            Results.Clear();

            TaskResult<IEnumerable<YouTubeVideo>> queryResponse = _youTubeApiService.QueryManyVideos(query).AsResult();

            yield return queryResponse;

            Results.AddRange(queryResponse.Result.Select(_videoFactory.MakeMatchedVideoViewModel));
        }
    }
}