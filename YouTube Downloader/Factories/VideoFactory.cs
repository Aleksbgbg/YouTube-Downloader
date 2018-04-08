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
            videoViewModel.Initialise(video);

            return videoViewModel;
        }

        public IMatchedVideoViewModel MakeMatchedVideoViewModel(YouTubeVideo video)
        {
            return MakeMatchedVideoViewModel(MakeVideoViewModel(video));
        }

        public IMatchedVideoViewModel MakeMatchedVideoViewModel(IVideoViewModel videoViewModel)
        {
            IMatchedVideoViewModel matchedVideoViewModel = IoC.Get<IMatchedVideoViewModel>();
            matchedVideoViewModel.Initialise(videoViewModel);

            return matchedVideoViewModel;
        }
    }
}