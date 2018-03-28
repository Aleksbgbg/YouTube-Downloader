namespace YouTube.Downloader.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using YouTube.Downloader.Models;

    internal interface IYouTubeApiService
    {
        Task<IEnumerable<YouTubeVideo>> GetVideos(string playlistId);
    }
}