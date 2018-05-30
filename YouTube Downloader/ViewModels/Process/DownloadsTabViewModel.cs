namespace YouTube.Downloader.ViewModels.Process
{
    using YouTube.Downloader.Core;
    using YouTube.Downloader.ViewModels.Interfaces.Process;

    internal class DownloadsTabViewModel : ProcessTabViewModel, IDownloadsTabViewModel
    {
        public DownloadsTabViewModel() : base(processTransferType => processTransferType == ProcessTransferType.Download)
        {
        }
    }
}