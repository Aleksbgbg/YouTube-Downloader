namespace YouTube.Downloader.Factories.Interfaces
{
    using YouTube.Downloader.Models;
    using YouTube.Downloader.ViewModels.Interfaces;

    internal interface IRequeryFactory
    {
        IRequeryViewModel MakeRequeryViewModel(IVideoViewModel videoViewModel, QueryResult queryResult);
    }
}