namespace YouTube.Downloader.Services
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;

    using Google.Apis.Auth.OAuth2;
    using Google.Apis.Services;
    using Google.Apis.YouTube.v3;
    using Google.Apis.YouTube.v3.Data;

    using YouTube.Downloader.Extensions;
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

        public async Task<IEnumerable<YouTubeVideo>> QueryVideos(string query)
        {
            // Types of query:
            // == Video ==
            // 1. Video title               Ed Sheeran - Perfect (Official Music Video)
            // 2. Full video URL            https://www.youtube.com/watch?v=2Vv-BfVoq4g
            // 3. Video ID                  2Vv-BfVoq4g
            //
            // == Playlist ==
            // 4. Full playlist video URL   https://www.youtube.com/watch?v=2Vv-BfVoq4g&list=PLx0sYbCqOb8TBPRdmBHs5Iftvv9TPboYG
            // 5. Full playlist URL         https://www.youtube.com/playlist?list=PLx0sYbCqOb8TBPRdmBHs5Iftvv9TPboYG
            // 6. Playlist ID               PLx0sYbCqOb8TBPRdmBHs5Iftvv9TPboYG

            // Query type 4
            {
                Match queryMatch = Regex.Match(query, @"watch\?v=[^&]+.+list=(?<PlaylistId>[^&\n]+)");

                if (queryMatch.Success)
                {
                    return await GetPlaylistVideos(queryMatch.Groups["PlaylistId"].Value);
                }
            }

            SearchResource.ListRequest searchRequest = _youTubeApiService.Search.List("snippet");
            searchRequest.Q = query;
            searchRequest.MaxResults = 1;

            SearchListResponse searchResponse = await searchRequest.ExecuteAsync();

            SearchResult result = searchResponse.Items.SingleOrDefault();

            if (result != null)
            {
                if (result.Id.Kind.Contains("video")) // Query types 1, 2, 3
                {
                    return EnumerableExtensions.ToEnumerable(await GetVideo(result.Id.VideoId));
                }

                if (result.Id.Kind.Contains("playlist")) // Query types 5, 6
                {
                    return await GetPlaylistVideos(result.Id.PlaylistId);
                }
            }

            // Invalid query (for the time being)
            return Enumerable.Empty<YouTubeVideo>();
        }

        private async Task<IEnumerable<YouTubeVideo>> GetPlaylistVideos(string playlistId)
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
                    videos.Add(await GetVideo(playlistItem.Snippet.ResourceId.VideoId));
                }
            } while (pageToken != null);

            return videos;
        }

        private async Task<YouTubeVideo> GetVideo(string id)
        {
            VideosResource.ListRequest videoRequest = _youTubeApiService.Videos.List("snippet");
            videoRequest.Id = id;

            VideoListResponse videoResponse = await videoRequest.ExecuteAsync();

            return new YouTubeVideo(videoResponse.Items[0]);
        }
    }
}