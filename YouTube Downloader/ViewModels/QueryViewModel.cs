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

        public QueryViewModel(IVideoCollectionViewModel videoCollectionViewModel, IYouTubeApiService youTubeApiService)
        {
            VideoCollectionViewModel = videoCollectionViewModel;
            _youTubeApiService = youTubeApiService;
        }

        public IVideoCollectionViewModel VideoCollectionViewModel { get; }

        public IEnumerable<IResult> Search(string query)
        {
            TaskResult<IEnumerable<YouTubeVideo>> getVideos = _youTubeApiService.GetVideos(query).AsResult();

            yield return getVideos;

            VideoCollectionViewModel.Load(getVideos.Result);
        }
    }
}