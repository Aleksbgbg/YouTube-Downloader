namespace YouTube.Downloader.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;

    using Google.Apis.Auth.OAuth2;
    using Google.Apis.Services;
    using Google.Apis.YouTube.v3;
    using Google.Apis.YouTube.v3.Data;

    using Newtonsoft.Json;

    using YouTube.Downloader.Extensions;
    using YouTube.Downloader.Models;
    using YouTube.Downloader.Services.Interfaces;

    internal sealed class YouTubeApiService : IYouTubeApiService
    {
        private const int MaxQueryResults = 50;

        private readonly YouTubeService _youTubeApiService;

        private readonly HttpClient _httpClient = new HttpClient();

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

        ~YouTubeApiService()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async Task<IEnumerable<QueryResult>[]> QueryVideos(IEnumerable<QueryResult> queries)
        {
            return await Task.WhenAll(queries.Select(QueryVideosPrivate));
        }

        private async Task<IEnumerable<QueryResult>> QueryVideosPrivate(QueryResult query)
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
                Match queryMatch = Regex.Match(query.Query, @"watch\?v=[^&]+.+list=(?<PlaylistId>[^&\n]+)");

                if (queryMatch.Success)
                {
                    return (await GetPlaylistVideos(queryMatch.Groups["PlaylistId"].Value)).Select(video => new QueryResult(query, video));
                }
            }

            SearchResource.ListRequest searchRequest = _youTubeApiService.Search.List("snippet");
            searchRequest.Q = query.Query;
            searchRequest.MaxResults = 1;

            SearchListResponse searchResponse = await searchRequest.ExecuteAsync();

            SearchResult result = searchResponse.Items.SingleOrDefault();

            if (result != null)
            {
                if (result.Id.Kind.Contains("video")) // Query types 1, 2, 3
                {
                    return new QueryResult(query, await GetVideo(result.Id.VideoId)).ToEnumerable();
                }

                if (result.Id.Kind.Contains("playlist")) // Query types 5, 6
                {
                    return (await GetPlaylistVideos(result.Id.PlaylistId)).Select(video => new QueryResult(query, video));
                }
            }

            // Invalid query (for the time being)
            return Enumerable.Empty<QueryResult>();
        }

        public async Task<IEnumerable<QueryResult>> QueryManyVideos(QueryResult query)
        {
            SearchResource.ListRequest searchRequest = _youTubeApiService.Search.List("snippet");
            searchRequest.Q = query.Query;
            searchRequest.MaxResults = MaxQueryResults;

            SearchListResponse searchResponse = await searchRequest.ExecuteAsync();

            return (await GetVideos(searchResponse.Items.Where(video => video.Id.Kind.Contains("video")).Select(video => video.Id.VideoId))).Select(video => new QueryResult(query, video));
        }

        private async Task<IEnumerable<YouTubeVideo>> GetPlaylistVideos(string playlistId)
        {
            List<YouTubeVideo> videos = new List<YouTubeVideo>();

            string pageToken = null;

            do
            {
                PlaylistItemsResource.ListRequest playlistRequest = _youTubeApiService.PlaylistItems.List("snippet");
                playlistRequest.PlaylistId = playlistId;
                playlistRequest.MaxResults = MaxQueryResults;
                playlistRequest.PageToken = pageToken;

                PlaylistItemListResponse playlistResponse = await playlistRequest.ExecuteAsync();

                pageToken = playlistResponse.NextPageToken;

                videos.AddRange(await GetVideos(playlistResponse.Items.Select(playlistItem => playlistItem.Snippet.ResourceId.VideoId)));
            } while (pageToken != null);

            return videos;
        }

        private async Task<IEnumerable<YouTubeVideo>> GetVideos(IEnumerable<string> videoIds)
        {
            return await Task.WhenAll(videoIds.Select(GetVideo));
        }

        private async Task<YouTubeVideo> GetVideo(string id)
        {
            VideosResource.ListRequest videoRequest = _youTubeApiService.Videos.List("snippet");
            videoRequest.Id = id;

            VideoListResponse videoResponse = await videoRequest.ExecuteAsync();

            Video video = videoResponse.Items.First();

            string statistics = await _httpClient.GetStringAsync($"https://www.googleapis.com/youtube/v3/videos?id={video.Id}&key=AIzaSyAIj7OmVIu1pQYstco3V8EUh1Bo_PCqIPE&part=statistics");

            return new YouTubeVideo(video, JsonConvert.DeserializeObject<VideoListResponse>(statistics)
                                                      .Items
                                                      .Single()
                                                      .Statistics
                                                      .ViewCount);
        }

        private void Dispose(bool disposing)
        {
            if (!disposing) return;

            _youTubeApiService.Dispose();
            _httpClient.Dispose();
        }
    }
}