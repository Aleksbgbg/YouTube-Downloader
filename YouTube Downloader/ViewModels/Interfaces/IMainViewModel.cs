namespace YouTube.Downloader.ViewModels.Interfaces
{
    internal interface IMainViewModel : IViewModelBase
    {
        IQueryViewModel QueryViewModel { get; }
    }
}