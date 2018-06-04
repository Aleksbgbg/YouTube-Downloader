namespace YouTube.Downloader.Services.Interfaces
{
    using System.Collections.Generic;

    using YouTube.Downloader.ViewModels.Interfaces;

    internal interface IProcessDispatcherService
    {
        void Dispatch(IEnumerable<IViewModelBase> viewModels);

        void Dispatch(IViewModelBase viewModel);
    }
}