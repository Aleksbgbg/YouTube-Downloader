namespace YouTube.Downloader.ViewModels.Interfaces
{
    using Caliburn.Micro;

    internal interface IRequeryViewModel : IViewModelBase
    {
        IObservableCollection<IMatchedVideoViewModel> Results { get; }

        string Query { get; set; }
    }
}