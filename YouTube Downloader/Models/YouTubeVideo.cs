namespace YouTube.Downloader.Models
{
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;

    using Google.Apis.YouTube.v3.Data;

    using Newtonsoft.Json;

    internal class YouTubeVideo
    {
        internal YouTubeVideo(PlaylistItem playlistItem)
        {
            Id = playlistItem.Id;
            Title = playlistItem.Snippet.Title;
            Channel = playlistItem.Snippet.ChannelTitle;
            Description = playlistItem.Snippet.Description;
            DateUploaded = playlistItem.Snippet.PublishedAt ?? new DateTime();
        }

        public string Id { get; }

        public string Title { get; }

        public string Channel { get; }

        public DateTime DateUploaded { get; }

        public string Description { get; }

        public ulong Views { get; private set; }

        internal async Task LoadViews()
        {
            using (HttpClient httpClient = new HttpClient())
            {
                string jsonString = await httpClient.GetStringAsync($"https://www.googleapis.com/youtube/v3/videos?id={Id}&key=AIzaSyAL-OrAZ6gIEUxOObwvLgjSiMxXXf8TDYw&part=statistics");

                Views = JsonConvert.DeserializeObject<VideoListResponse>(jsonString)
                                   .Items
                                   .Single()
                                   .Statistics
                                   .ViewCount ?? 0;
            }
        }
    }
}