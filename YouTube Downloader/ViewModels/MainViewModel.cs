namespace YouTube.Downloader.ViewModels
{
    using YouTube.Downloader.ViewModels.Interfaces;
    using YouTube.Downloader.ViewModels.Interfaces.Process;

    internal class MainViewModel : ViewModelBase, IMainViewModel
    {
        public MainViewModel(IQueryViewModel queryViewModel, IProcessTabsViewModel processTabsViewModel)
        {
            QueryViewModel = queryViewModel;
            ProcessTabsViewModel = processTabsViewModel;
        }

        public IQueryViewModel QueryViewModel { get; }

        public IProcessTabsViewModel ProcessTabsViewModel { get; }
    }
}