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

        public IEnumerable<IResult> Search(string queryLines)
        {
            IsLoading = true;

            MatchedVideosViewModel.Clear();

            foreach (TaskResult<IEnumerable<YouTubeVideo>> queryResult in queryLines
                                                                          .Split('\n')
                                                                          .Where(line => !string.IsNullOrWhiteSpace(line))
                                                                          .Select(query => _youTubeApiService.QueryVideos(query).AsResult()))
            {
                yield return queryResult;

                MatchedVideosViewModel.Onload(queryResult.Result);
            }

            IsLoading = false;
        }

        public void Download()
        {
            MatchedVideosViewModel.DownloadSelected();
        }
    }
}