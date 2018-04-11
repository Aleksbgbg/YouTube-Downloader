namespace YouTube.Downloader.Models
{
    using System;

    using Caliburn.Micro;

    using Google.Apis.YouTube.v3.Data;

    internal class YouTubeVideo : PropertyChangedBase
    {
        internal YouTubeVideo(Video video, ulong? views) : this(video.Id, video.Snippet.Title, video.Snippet.ChannelTitle,
                                                                video.Snippet.Description, video.Snippet.PublishedAt, views)
        {
        }

        private YouTubeVideo(string id, string title, string channel, string description, DateTime? dateUploaded, ulong? views)
        {
            Id = id;
            Title = title;
            Channel = channel;
            Description = description;

            if (dateUploaded.HasValue)
            {
                DateUploaded = dateUploaded.Value;
            }

            if (views.HasValue)
            {
                Views = views.Value;
            }
        }

        public string Id { get; }

        public string Title { get; }

        public string Channel { get; }

        public DateTime DateUploaded { get; }

        public string Description { get; }

        public ulong Views { get; }
    }
}