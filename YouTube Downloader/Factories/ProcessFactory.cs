namespace YouTube.Downloader.Factories
{
    using Caliburn.Micro;

    using YouTube.Downloader.Core.Downloading;
    using YouTube.Downloader.Factories.Interfaces;
    using YouTube.Downloader.Models;
    using YouTube.Downloader.Services.Interfaces;
    using YouTube.Downloader.ViewModels.Interfaces;
    using YouTube.Downloader.ViewModels.Interfaces.Process;

    internal class ProcessFactory : IProcessFactory
    {
        private readonly Settings _settings;

        public ProcessFactory(ISettingsService settingsService)
        {
            _settings = settingsService.Settings;
        }

        public IProcessViewModel MakeProcessViewModel(IVideoViewModel videoViewModel)
        {
            IProcessViewModel processViewModel = IoC.Get<IProcessViewModel>();
            processViewModel.Initialise(videoViewModel, new DownloadProcess(processViewModel.DownloadStatus, videoViewModel.Video, _settings));

            return processViewModel;
        }
    }
}