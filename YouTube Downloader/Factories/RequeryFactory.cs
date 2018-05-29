namespace YouTube.Downloader.Factories
{
    using Caliburn.Micro;

    using YouTube.Downloader.Factories.Interfaces;
    using YouTube.Downloader.Models;
    using YouTube.Downloader.ViewModels.Interfaces;

    internal class RequeryFactory : IRequeryFactory
    {
        public IRequeryViewModel MakeRequeryViewModel(IVideoViewModel videoViewModel, QueryResult queryResult)
        {
            IRequeryViewModel requeryViewModel = IoC.Get<IRequeryViewModel>();
            requeryViewModel.Initialise(videoViewModel, queryResult);

            return requeryViewModel;
        }
    }
}