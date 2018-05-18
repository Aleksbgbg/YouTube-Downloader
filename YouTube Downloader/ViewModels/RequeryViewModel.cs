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

        private IVideoViewModel _requeryTarget;

        public RequeryViewModel(IVideoFactory videoFactory, IYouTubeApiService youTubeApiService)
        {
            DisplayName = "Requery Video";

            _videoFactory = videoFactory;
            _youTubeApiService = youTubeApiService;
        }

        public IObservableCollection<IMatchedVideoViewModel> Results { get; } = new BindableCollection<IMatchedVideoViewModel>();

        private string _query;
        public string Query
        {
            get => _query;

            set
            {
                if (_query == value) return;

                _query = value;
                NotifyOfPropertyChange(() => Query);

                DisplayName = Query;
            }
        }

        public IEnumerable<IResult> Search()
        {
            Results.Clear();

            TaskResult<IEnumerable<YouTubeVideo>> queryResponse = _youTubeApiService.QueryManyVideos(Query).AsResult();

            yield return queryResponse;

            Results.AddRange(queryResponse.Result.Select(_videoFactory.MakeMatchedVideoViewModel));
        }

        public void Initialise(IVideoViewModel requeryTarget)
        {
            _requeryTarget = requeryTarget;
            Query = requeryTarget.Video.Title;

            Coroutine.BeginExecute(Search().GetEnumerator());
        }
    }
}