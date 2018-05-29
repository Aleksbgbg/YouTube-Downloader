namespace YouTube.Downloader.Factories
{
    using Caliburn.Micro;

    using YouTube.Downloader.Factories.Interfaces;
    using YouTube.Downloader.Models;
    using YouTube.Downloader.ViewModels.Interfaces;

    internal class VideoFactory : IVideoFactory
    {
        public IVideoViewModel MakeVideoViewModel(YouTubeVideo video)
        {
            IVideoViewModel videoViewModel = IoC.Get<IVideoViewModel>();
            videoViewModel.Video = video;

            return videoViewModel;
        }

        public IMatchedVideoViewModel MakeMatchedVideoViewModel(QueryResult queryResult)
        {
            IMatchedVideoViewModel matchedVideoViewModel = IoC.Get<IMatchedVideoViewModel>();
            matchedVideoViewModel.Initialise(MakeVideoViewModel(queryResult.MatchedVideo), queryResult);

            return matchedVideoViewModel;
        }
    }
}