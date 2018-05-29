namespace YouTube.Downloader.Services.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using YouTube.Downloader.Models;

    internal interface IYouTubeApiService : IDisposable
    {
        Task<IEnumerable<QueryResult>[]> QueryVideos(IEnumerable<QueryResult> query);

        Task<IEnumerable<QueryResult>> QueryManyVideos(QueryResult query);
    }
}