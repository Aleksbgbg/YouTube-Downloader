namespace YouTube.Downloader.ViewModels.Process
{
    using System.Linq;

    using Caliburn.Micro;

    using YouTube.Downloader.Core;
    using YouTube.Downloader.Core.Downloading;
    using YouTube.Downloader.Extensions;
    using YouTube.Downloader.Services.Interfaces;
    using YouTube.Downloader.ViewModels.Interfaces.Process;

    internal class DownloadsTabViewModel : ProcessTabViewModel, IDownloadsTabViewModel
    {
        private readonly IEventAggregator _eventAggregator;

        private readonly IDownloadService _downloadService;

        public DownloadsTabViewModel(IEventAggregator eventAggregator, IDownloadService downloadService) : base(eventAggregator, processTransferType => processTransferType == ProcessTransferType.Download)
        {
            _eventAggregator = eventAggregator;
            _downloadService = downloadService;
        }

        private protected override void OnProcessesAdded(IProcessViewModel[] processViewModels)
        {
            _downloadService.QueueDownloads(processViewModels.Select(processViewModel => processViewModel.Process).Cast<DownloadProcess>());
        }

        private protected override void OnProcessExited(IProcessViewModel processViewModel)
        {
            _eventAggregator.BeginPublishOnUIThread(new ProcessTransferMessage(ProcessTransferType.Convert, processViewModel.ToEnumerable()));
        }
    }
}