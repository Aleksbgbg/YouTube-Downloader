namespace YouTube.Downloader.ViewModels
{
    using System.Collections.Generic;

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

        public IEnumerable<IResult> Search(string query)
        {
            IsLoading = true;

            TaskResult<IEnumerable<YouTubeVideo>> getVideos = _youTubeApiService.GetVideos(query).AsResult();

            yield return getVideos;

            MatchedVideosViewModel.Load(getVideos.Result);

            IsLoading = false;
        }

        public void Download()
        {
            MatchedVideosViewModel.DownloadSelected();
        }
    }
}