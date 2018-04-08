namespace YouTube.Downloader.Factories
{
    using Caliburn.Micro;

    using YouTube.Downloader.Factories.Interfaces;
    using YouTube.Downloader.ViewModels.Interfaces;

    internal class DownloadFactory : IDownloadFactory
    {
        public IDownloadViewModel MakeDownloadViewModel(IVideoViewModel videoViewModel)
        {
            IDownloadViewModel downloadViewModel = IoC.Get<IDownloadViewModel>();
            downloadViewModel.Initialise(videoViewModel);

            return downloadViewModel;
        }
    }
}