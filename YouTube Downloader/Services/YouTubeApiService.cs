namespace YouTube.Downloader.Services
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Google.Apis.Auth.OAuth2;
    using Google.Apis.Services;
    using Google.Apis.YouTube.v3;
    using Google.Apis.YouTube.v3.Data;

    using YouTube.Downloader.Models;
    using YouTube.Downloader.Services.Interfaces;

    internal class YouTubeApiService : IYouTubeApiService
    {
        private readonly YouTubeService _youTubeApiService;

        public YouTubeApiService()
        {
            using (FileStream clientSecrets = File.OpenRead("Resources/Client Secret.json"))
            {
                _youTubeApiService = new YouTubeService(new BaseClientService.Initializer
                {
                        HttpClientInitializer = GoogleWebAuthorizationBroker.AuthorizeAsync(GoogleClientSecrets.Load(clientSecrets).Secrets,
                                                                                            new string[] { YouTubeService.Scope.YoutubeReadonly },
                                                                                            "user",
                                                                                            CancellationToken.None).Result
                });
            }
        }

        public async Task<IEnumerable<YouTubeVideo>> GetVideos(string playlistId)
        {
            List<YouTubeVideo> videos = new List<YouTubeVideo>();

            string pageToken = null;

            do
            {
                PlaylistItemsResource.ListRequest playlistRequest = _youTubeApiService.PlaylistItems.List("snippet");
                playlistRequest.PlaylistId = playlistId;
                playlistRequest.MaxResults = 50;
                playlistRequest.PageToken = pageToken;

                PlaylistItemListResponse playlistResponse = await playlistRequest.ExecuteAsync();

                pageToken = playlistResponse.NextPageToken;

                foreach (PlaylistItem playlistItem in playlistResponse.Items)
                {
                    VideosResource.ListRequest videoRequest = _youTubeApiService.Videos.List("snippet");
                    videoRequest.Id = playlistItem.Snippet.ResourceId.VideoId;

                    VideoListResponse videoResponse = await videoRequest.ExecuteAsync();

                    videos.Add(new YouTubeVideo(videoResponse.Items[0]));
                }
            } while (pageToken != null);

            return videos;
        }
    }
}