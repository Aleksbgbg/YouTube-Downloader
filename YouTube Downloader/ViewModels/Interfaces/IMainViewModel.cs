namespace YouTube.Downloader.ViewModels.Interfaces
{
    using YouTube.Downloader.ViewModels.Interfaces.Process;

    internal interface IMainViewModel : IViewModelBase
    {
        IQueryViewModel QueryViewModel { get; }

        IProcessTabsViewModel ProcessTabsViewModel { get; }
    }
}