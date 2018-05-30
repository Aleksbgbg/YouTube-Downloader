namespace YouTube.Downloader.Factories.Interfaces
{
    using YouTube.Downloader.ViewModels.Interfaces;
    using YouTube.Downloader.ViewModels.Interfaces.Process;

    internal interface IProcessFactory
    {
        IProcessViewModel MakeProcessViewModel(IVideoViewModel videoViewModel);
    }
}