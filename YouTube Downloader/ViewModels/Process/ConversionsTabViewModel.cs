namespace YouTube.Downloader.ViewModels.Process
{
    using YouTube.Downloader.Core;
    using YouTube.Downloader.ViewModels.Interfaces.Process;

    internal class ConversionsTabViewModel : ProcessTabViewModel, IConversionsTabViewModel
    {
        public ConversionsTabViewModel() : base(processTransferType => processTransferType == ProcessTransferType.Convert)
        {
        }
    }
}