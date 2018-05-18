namespace YouTube.Downloader.Factories
{
    using Caliburn.Micro;

    using YouTube.Downloader.Factories.Interfaces;
    using YouTube.Downloader.ViewModels.Interfaces;

    internal class RequeryFactory : IRequeryFactory
    {
        public IRequeryViewModel MakeRequeryViewModel(IVideoViewModel videoViewModel)
        {
            IRequeryViewModel requeryViewModel = IoC.Get<IRequeryViewModel>();
            requeryViewModel.Initialise(videoViewModel);

            return requeryViewModel;
        }
    }
}