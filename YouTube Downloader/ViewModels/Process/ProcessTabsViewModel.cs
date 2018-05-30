namespace YouTube.Downloader.ViewModels.Process
{
    using Caliburn.Micro;

    using YouTube.Downloader.ViewModels.Interfaces.Process;

    internal class ProcessTabsViewModel : Conductor<IProcessTabViewModel>.Collection.OneActive, IProcessTabsViewModel
    {
        public ProcessTabsViewModel(
                IDownloadsTabViewModel downloadsTabViewModel,
                IConversionsTabViewModel conversionsTabViewModel,
                ICompletedTabViewModel completedTabViewModel
                )
        {
            Items.Add(downloadsTabViewModel);
            Items.Add(conversionsTabViewModel);
            Items.Add(completedTabViewModel);
        }
    }
}