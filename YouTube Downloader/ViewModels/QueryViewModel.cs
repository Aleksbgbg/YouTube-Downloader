namespace YouTube.Downloader.ViewModels
{
    using System.Collections.Generic;
    using System.Linq;

    using Caliburn.Micro;

    using YouTube.Downloader.Models;
    using YouTube.Downloader.Services.Interfaces;
    using YouTube.Downloader.ViewModels.Interfaces;

    internal class QueryViewModel : ViewModelBase, IQueryViewModel
    {
        private readonly IYouTubeApiService _youTubeApiService;

        public QueryViewModel(IMatchedVideosViewModel matchedVideosViewModel, IYouTubeApiService youTubeApiService)
        {
            MatchedVideosViewModel = matchedVideosViewModel;
            _youTubeApiService = youTubeApiService;
        }

        public IMatchedVideosViewModel MatchedVideosViewModel { get; }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;

            private set
            {
                if (_isLoading == value) return;

                _isLoading = value;
                NotifyOfPropertyChange(() => IsLoading);
            }
        }

        private bool _queryBoxIsExpanded;
        public bool QueryBoxIsExpanded
        {
            get => _queryBoxIsExpanded;

            private set
            {
                if (_queryBoxIsExpanded == value || !value && Query.Contains('\n')) return;

                _queryBoxIsExpanded = value;
                NotifyOfPropertyChange(() => QueryBoxIsExpanded);
            }
        }

        private string _query;
        public string Query
        {
            get => _query;

            set
            {
                if (_query == value) return;

                _query = value;
                NotifyOfPropertyChange(() => Query);

                if (_query.Contains('\n'))
                {
                    QueryBoxIsExpanded = true;
                }
            }
        }

        public IEnumerable<IResult> Search()
        {
            IsLoading = true;

            MatchedVideosViewModel.Clear();

            foreach (TaskResult<IEnumerable<YouTubeVideo>> queryResult in Query
                                                                          .Split('\n')
                                                                          .Where(line => !string.IsNullOrWhiteSpace(line))
                                                                          .Select(query => query.Trim())
                                                                          .Select(query => _youTubeApiService.QueryVideos(query).AsResult()))
            {
                yield return queryResult;

                MatchedVideosViewModel.Onload(queryResult.Result);
            }

            IsLoading = false;
        }

        public void ToggleQueryBoxState()
        {
            QueryBoxIsExpanded = !QueryBoxIsExpanded;
        }

        public void Download()
        {
            MatchedVideosViewModel.DownloadSelected();
        }
    }
}