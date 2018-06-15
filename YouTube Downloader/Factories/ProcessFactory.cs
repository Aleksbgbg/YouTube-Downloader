namespace YouTube.Downloader.Factories
{
    using Caliburn.Micro;

    using YouTube.Downloader.Core.Downloading;
    using YouTube.Downloader.Factories.Interfaces;
    using YouTube.Downloader.Models;
    using YouTube.Downloader.Models.Download;
    using YouTube.Downloader.Services.Interfaces;
    using YouTube.Downloader.ViewModels.Interfaces;
    using YouTube.Downloader.ViewModels.Interfaces.Process;
    using YouTube.Downloader.ViewModels.Interfaces.Process.Entities;

    internal class ProcessFactory : IProcessFactory
    {
        private readonly Settings _settings;

        public ProcessFactory(ISettingsService settingsService)
        {
            _settings = settingsService.Settings;
        }

        public IDownloadProcessViewModel MakeDownloadProcessViewModel(IVideoViewModel videoViewModel)
        {
            IDownloadProcessViewModel downloadProcessViewModel = IoC.Get<IDownloadProcessViewModel>();

            DownloadProgress downloadProgress = new DownloadProgress();

            downloadProcessViewModel.Initialise(videoViewModel, new DownloadProcess(downloadProgress, videoViewModel.Video, _settings), downloadProgress);

            return downloadProcessViewModel;
        }

        public IConvertProcessViewModel MakeConvertProcessViewModel(IVideoViewModel videoViewModel, ConvertProcess process, ConvertProgress progress)
        {
            IConvertProcessViewModel convertProcessViewModel = IoC.Get<IConvertProcessViewModel>();
            convertProcessViewModel.Initialise(videoViewModel, process, progress);

            return convertProcessViewModel;
        }

        public ICompleteProcessViewModel MakeCompleteProcessViewModel(IVideoViewModel videoViewModel, DownloadState downloadState)
        {
            ICompleteProcessViewModel completeProcessViewModel = IoC.Get<ICompleteProcessViewModel>();
            completeProcessViewModel.Initialise(videoViewModel, downloadState);

            return completeProcessViewModel;
        }
    }
}