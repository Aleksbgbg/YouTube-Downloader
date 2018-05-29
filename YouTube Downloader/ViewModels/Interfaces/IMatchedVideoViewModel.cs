namespace YouTube.Downloader.ViewModels.Interfaces
{
    using YouTube.Downloader.Models;

    internal interface IMatchedVideoViewModel : IViewModelBase
    {
        IVideoViewModel VideoViewModel { get; }

        QueryResult QueryResult { get; }

        void Initialise(IVideoViewModel videoViewModel, QueryResult queryResult);
    }
}