namespace YouTube.Downloader.ViewModels
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

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
                if (_queryBoxIsExpanded == value) return;

                _queryBoxIsExpanded = value;
                NotifyOfPropertyChange(() => QueryBoxIsExpanded);
                NotifyOfPropertyChange(() => CanToggleQueryBoxState);
            }
        }

        private string _query = string.Empty;
        public string Query
        {
            get => _query;

            set
            {
                if (_query == value) return;

                _query = value;
                NotifyOfPropertyChange(() => Query);
                NotifyOfPropertyChange(() => CanToggleQueryBoxState);

                if (_query.Contains('\n'))
                {
                    QueryBoxIsExpanded = true;
                }
            }
        }

        public bool CanToggleQueryBoxState => !QueryBoxIsExpanded || QueryBoxIsExpanded && !Query.Contains('\n');

        public IEnumerable<IResult> Search()
        {
            IsLoading = true;

            TaskResult<IEnumerable<YouTubeVideo>[]> tasks = Task.WhenAll(Query
                                                                        .Split('\n')
                                                                        .Where(line => !string.IsNullOrWhiteSpace(line))
                                                                        .Select(query => query.Trim())
                                                                        .Select(_youTubeApiService.QueryVideos))
                                                                .AsResult();

            yield return tasks;

            MatchedVideosViewModel.Load(tasks.Result.SelectMany(enumerable => enumerable));

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