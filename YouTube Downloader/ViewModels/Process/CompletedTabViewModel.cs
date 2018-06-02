namespace YouTube.Downloader.ViewModels.Process
{
    using Caliburn.Micro;

    using YouTube.Downloader.Core;
    using YouTube.Downloader.ViewModels.Interfaces.Process;

    internal class CompletedTabViewModel : ProcessTabViewModel, ICompletedTabViewModel
    {
        public CompletedTabViewModel(IEventAggregator eventAggregator)
                : base(eventAggregator, null, processTransferType => processTransferType == ProcessTransferType.Complete)
        {
        }
    }
}