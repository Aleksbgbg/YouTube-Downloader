namespace YouTube.Downloader.ViewModels
{
    using YouTube.Downloader.ViewModels.Interfaces;

    internal class MainViewModel : ViewModelBase, IMainViewModel
    {
        public MainViewModel(IQueryViewModel queryViewModel, ICurrentDownloadsViewModel currentDownloadsViewModel)
        {
            QueryViewModel = queryViewModel;
            CurrentDownloadsViewModel = currentDownloadsViewModel;
        }

        public IQueryViewModel QueryViewModel { get; }

        public ICurrentDownloadsViewModel CurrentDownloadsViewModel { get; }
    }
}