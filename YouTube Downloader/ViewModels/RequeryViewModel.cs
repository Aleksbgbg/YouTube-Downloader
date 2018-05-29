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
            _videoFactory = videoFactory;
            _youTubeApiService = youTubeApiService;

            Query = string.Empty;
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

                DisplayName = _query == string.Empty ? "Requery Video" : Query;
            }
        }

        private IMatchedVideoViewModel _selectedMatch;
        public IMatchedVideoViewModel SelectedMatch
        {
            get => _selectedMatch;

            set
            {
                if (_selectedMatch == value) return;

                _selectedMatch = value;
                NotifyOfPropertyChange(() => SelectedMatch);

                NotifyOfPropertyChange(() => CanApply);

                if (_selectedMatch != null)
                {
                    NewVideo = SelectedMatch.VideoViewModel;
                }
            }
        }

        private IVideoViewModel _newVideo;
        public IVideoViewModel NewVideo
        {
            get => _newVideo;

            private set
            {
                if (_newVideo == value) return;

                _newVideo = value;
                NotifyOfPropertyChange(() => NewVideo);
            }
        }

        public bool CanApply => SelectedMatch != null;

        public void Apply()
        {
            _requeryTarget.Video = SelectedMatch.VideoViewModel.Video;
            TryClose();
        }

        public IEnumerable<IResult> Search()
        {
            Results.Clear();

            TaskResult<IEnumerable<QueryResult>> queryResponse = _youTubeApiService.QueryManyVideos(new QueryResult(Query)).AsResult();

            yield return queryResponse;

            Results.AddRange(queryResponse.Result.Select(_videoFactory.MakeMatchedVideoViewModel));
        }

        public void Initialise(IVideoViewModel requeryTarget, QueryResult queryResult)
        {
            _requeryTarget = requeryTarget;
            Query = queryResult.Query;

            NewVideo = _videoFactory.MakeVideoViewModel(requeryTarget.Video);

            Coroutine.BeginExecute(Search().GetEnumerator());
        }
    }
}