namespace YouTube.Downloader.ViewModels.Interfaces
{
    internal interface IQueryViewModel : IViewModelBase
    {
        IVideoCollectionViewModel VideoCollectionViewModel { get; }
    }
}