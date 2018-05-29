namespace YouTube.Downloader.Factories.Interfaces
{
    using YouTube.Downloader.Models;
    using YouTube.Downloader.ViewModels.Interfaces;

    internal interface IVideoFactory
    {
        IVideoViewModel MakeVideoViewModel(YouTubeVideo video);

        IMatchedVideoViewModel MakeMatchedVideoViewModel(QueryResult queryResult);
    }
}