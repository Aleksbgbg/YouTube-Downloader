namespace YouTube.Downloader.Models
{
    using System;

    using Caliburn.Micro;

    using Google.Apis.YouTube.v3.Data;

    using Newtonsoft.Json;

    internal class YouTubeVideo : PropertyChangedBase
    {
        internal YouTubeVideo(Video video, ulong? views) : this(video.Id, video.Snippet.Title, video.Snippet.ChannelTitle, video.Snippet.PublishedAt, views)
        {
        }

        [JsonConstructor]
        internal YouTubeVideo(string id, string title, string channel, DateTime? dateUploaded, ulong? views)
        {
            Id = id;
            Title = title;
            Channel = channel;

            if (dateUploaded.HasValue)
            {
                DateUploaded = dateUploaded.Value;
            }

            if (views.HasValue)
            {
                Views = views.Value;
            }
        }

        [JsonProperty("Id")]
        public string Id { get; }

        [JsonProperty("Title")]
        public string Title { get; }

        [JsonProperty("Channel")]
        public string Channel { get; }

        [JsonProperty("DateUploaded")]
        public DateTime DateUploaded { get; }

        [JsonProperty("Views")]
        public ulong Views { get; }
    }
}