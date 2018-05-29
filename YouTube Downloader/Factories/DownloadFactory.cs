namespace YouTube.Downloader.Factories
{
    using Caliburn.Micro;

    using YouTube.Downloader.Core.Downloading;
    using YouTube.Downloader.Factories.Interfaces;
    using YouTube.Downloader.Models;
    using YouTube.Downloader.Services.Interfaces;
    using YouTube.Downloader.ViewModels.Interfaces;

    internal class DownloadFactory : IDownloadFactory
    {
        private readonly Settings _settings;

        public DownloadFactory(ISettingsService settingsService)
        {
            _settings = settingsService.Settings;
        }

        public IDownloadViewModel MakeDownloadViewModel(IVideoViewModel videoViewModel)
        {
            IDownloadViewModel downloadViewModel = IoC.Get<IDownloadViewModel>();
            downloadViewModel.Initialise(videoViewModel, new DownloadProcess(downloadViewModel.DownloadStatus, videoViewModel.Video, _settings));

            return downloadViewModel;
        }
    }
}