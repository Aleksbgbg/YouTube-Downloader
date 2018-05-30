namespace YouTube.Downloader.ViewModels.Process
{
    using YouTube.Downloader.Core;
    using YouTube.Downloader.ViewModels.Interfaces.Process;

    internal class CompletedTabViewModel : ProcessTabViewModel, ICompletedTabViewModel
    {
        public CompletedTabViewModel() : base(processTransferType => processTransferType == ProcessTransferType.Complete)
        {
        }
    }
}