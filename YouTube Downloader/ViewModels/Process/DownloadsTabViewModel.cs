namespace YouTube.Downloader.ViewModels.Process
{
    using System.Linq;

    using Caliburn.Micro;

    using YouTube.Downloader.Core;
    using YouTube.Downloader.Core.Downloading;
    using YouTube.Downloader.Services.Interfaces;
    using YouTube.Downloader.ViewModels.Interfaces.Process;

    internal class DownloadsTabViewModel : ProcessTabViewModel, IDownloadsTabViewModel
    {
        private readonly IDownloadService _downloadService;

        public DownloadsTabViewModel(IEventAggregator eventAggregator, IDownloadService downloadService, IProcessDispatcherService processDispatcherService)
                : base(eventAggregator, processDispatcherService, processTransferType => processTransferType == ProcessTransferType.Download)
        {
            _downloadService = downloadService;
        }

        private protected override void OnProcessesAdded(IProcessViewModel[] processViewModels)
        {
            _downloadService.QueueDownloads(processViewModels.Select(processViewModel => processViewModel.Process).Cast<DownloadProcess>());
        }
    }
}