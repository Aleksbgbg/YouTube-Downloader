namespace YouTube.Downloader.ViewModels
{
    using YouTube.Downloader.ViewModels.Interfaces;

    internal class MainViewModel : ViewModelBase, IMainViewModel
    {
        public MainViewModel(IQueryViewModel queryViewModel)
        {
            QueryViewModel = queryViewModel;
        }

        public IQueryViewModel QueryViewModel { get; }
    }
}