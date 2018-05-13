namespace YouTube.Downloader.ViewModels
{
    using Caliburn.Micro;

    using YouTube.Downloader.ViewModels.Interfaces;

    internal class RequeryViewModel : ViewModelBase, IRequeryViewModel
    {
        public IObservableCollection<IMatchedVideoViewModel> Results { get; } = new BindableCollection<IMatchedVideoViewModel>();

        public void Search(string query)
        {
            Results.Clear();
        }
    }
}