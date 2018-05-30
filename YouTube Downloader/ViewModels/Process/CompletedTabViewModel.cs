namespace YouTube.Downloader.ViewModels.Process
{
    using Caliburn.Micro;

    using YouTube.Downloader.Core;
    using YouTube.Downloader.Models.Download;
    using YouTube.Downloader.ViewModels.Interfaces.Process;

    internal class CompletedTabViewModel : ProcessTabViewModel, ICompletedTabViewModel
    {
        public CompletedTabViewModel(IEventAggregator eventAggregator) : base(eventAggregator, processTransferType => processTransferType == ProcessTransferType.Complete)
        {
        }

        private protected override void OnProcessesAdded(IProcessViewModel[] processViewModels)
        {
            foreach (IProcessViewModel processViewModel in processViewModels)
            {
                processViewModel.DownloadStatus.DownloadState = DownloadState.Completed;
            }
        }
    }
}