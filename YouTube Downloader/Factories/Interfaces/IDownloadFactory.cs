namespace YouTube.Downloader.Factories.Interfaces
{
    using YouTube.Downloader.ViewModels.Interfaces;

    internal interface IDownloadFactory
    {
        IDownloadViewModel MakeDownloadViewModel(IVideoViewModel videoViewModel);
    }
}