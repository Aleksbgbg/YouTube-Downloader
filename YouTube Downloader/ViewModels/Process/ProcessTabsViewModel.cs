namespace YouTube.Downloader.ViewModels.Process
{
    using Caliburn.Micro;

    using YouTube.Downloader.ViewModels.Interfaces;
    using YouTube.Downloader.ViewModels.Interfaces.Process;
    using YouTube.Downloader.ViewModels.Interfaces.Process.Tabs;

    internal class ProcessTabsViewModel : Conductor<IViewModelBase>.Collection.OneActive, IProcessTabsViewModel
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