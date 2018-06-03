namespace YouTube.Downloader.Services.Interfaces
{
    using YouTube.Downloader.ViewModels.Interfaces.Process;

    internal interface IProcessDispatcherService
    {
        void Dispatch(IProcessViewModel[] processViewModels);

        void Dispatch(IProcessViewModel processViewModel);
    }
}