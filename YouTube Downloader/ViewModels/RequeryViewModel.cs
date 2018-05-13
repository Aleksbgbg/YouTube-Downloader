namespace YouTube.Downloader.ViewModels
{
    using Caliburn.Micro;

    using YouTube.Downloader.ViewModels.Interfaces;

    internal class RequeryViewModel : ViewModelBase, IRequeryViewModel
    {
        public IObservableCollection<IMatchedVideoViewModel> Results { get; } = new BindableCollection<IMatchedVideoViewModel>();

        private string _query;
        public string Query
        {
            get => _query;

            set
            {
                if (_query == value) return;

                _query = value;
                NotifyOfPropertyChange(() => Query);
            }
        }
    }
}