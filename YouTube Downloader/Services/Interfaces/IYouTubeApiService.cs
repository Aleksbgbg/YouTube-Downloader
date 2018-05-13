namespace YouTube.Downloader.Services.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using YouTube.Downloader.Models;

    internal interface IYouTubeApiService : IDisposable
    {
        Task<IEnumerable<YouTubeVideo>> QueryVideos(string query);

        Task<IEnumerable<YouTubeVideo>> QueryManyVideos(string query);
    }
}