namespace YouTube.Downloader.Services
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Google.Apis.Auth.OAuth2;
    using Google.Apis.Services;
    using Google.Apis.Util.Store;
    using Google.Apis.YouTube.v3;
    using Google.Apis.YouTube.v3.Data;

    using YouTube.Downloader.Models;
    using YouTube.Downloader.Services.Interfaces;

    internal class YouTubeApiService : IYouTubeApiService
    {
        private readonly YouTubeService _youTubeApiService;

        public YouTubeApiService()
        {
            using (FileStream clientSecrets = File.OpenRead("Client Secret.json"))
            {
                _youTubeApiService = new YouTubeService(new BaseClientService.Initializer
                {
                        HttpClientInitializer = GoogleWebAuthorizationBroker.AuthorizeAsync(GoogleClientSecrets.Load(clientSecrets).Secrets,
                                                                                            new string[] { YouTubeService.Scope.YoutubeReadonly },
                                                                                            "user",
                                                                                            CancellationToken.None,
                                                                                            new FileDataStore("YouTube Downloader")).Result
                });
            }
        }

        public async Task<IEnumerable<YouTubeVideo>> GetVideos(string playlistId)
        {
            List<YouTubeVideo> videos = new List<YouTubeVideo>();

            string pageToken = null;

            do
            {
                PlaylistItemsResource.ListRequest listPlaylistRequest = _youTubeApiService.PlaylistItems.List("snippet");
                listPlaylistRequest.PlaylistId = playlistId;
                listPlaylistRequest.MaxResults = 50;
                listPlaylistRequest.PageToken = pageToken;

                PlaylistItemListResponse response = await listPlaylistRequest.ExecuteAsync();

                pageToken = response.NextPageToken;

                videos.AddRange(response.Items.Select(video => new YouTubeVideo(video)));
            } while (pageToken != null);

            return videos;
        }
    }
}