namespace YouTube.Downloader.Factories.Interfaces
{
    using YouTube.Downloader.ViewModels.Interfaces;

    internal interface IRequeryFactory
    {
        IRequeryViewModel MakeRequeryViewModel(IVideoViewModel videoViewModel);
    }
}