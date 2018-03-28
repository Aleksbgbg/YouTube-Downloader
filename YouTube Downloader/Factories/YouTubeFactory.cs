namespace YouTube.Downloader.Factories
{
    using Caliburn.Micro;

    using YouTube.Downloader.Factories.Interfaces;
    using YouTube.Downloader.Models;
    using YouTube.Downloader.ViewModels.Interfaces;

    internal class YouTubeFactory : IYouTubeFactory
    {
        public IYouTubeVideoViewModel MakeVideoViewModel(YouTubeVideo video)
        {
            IYouTubeVideoViewModel videoViewModel = IoC.Get<IYouTubeVideoViewModel>();
            videoViewModel.Initialise(video);

            return videoViewModel;
        }
    }
}