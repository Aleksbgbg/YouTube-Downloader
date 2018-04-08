namespace YouTube.Downloader.Models
{
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;

    using Caliburn.Micro;

    using Google.Apis.YouTube.v3.Data;

    using Newtonsoft.Json;

    internal class YouTubeVideo : PropertyChangedBase
    {
        internal YouTubeVideo(Video video) : this(video.Id, video.Snippet.Title, video.Snippet.ChannelTitle, video.Snippet.Description, video.Snippet.PublishedAt)
        {
        }

        private YouTubeVideo(string id, string title, string channel, string description, DateTime? dateUploaded)
        {
            Id = id;
            Title = title;
            Channel = channel;
            Description = description;

            if (dateUploaded.HasValue)
            {
                DateUploaded = dateUploaded.Value;
            }

            Task.Run(LoadViews);
        }

        public string Id { get; }

        public string Title { get; }

        public string Channel { get; }

        public DateTime DateUploaded { get; }

        public string Description { get; }

        private ulong _views;
        public ulong Views
        {
            get => _views;

            private set
            {
                if (_views == value) return;

                _views = value;
                NotifyOfPropertyChange(() => Views);
            }
        }

        private async Task LoadViews()
        {
            using (HttpClient httpClient = new HttpClient())
            {
                string jsonString = await httpClient.GetStringAsync($"https://www.googleapis.com/youtube/v3/videos?id={Id}&key=AIzaSyAIj7OmVIu1pQYstco3V8EUh1Bo_PCqIPE&part=statistics");

                ulong? viewCount = JsonConvert.DeserializeObject<VideoListResponse>(jsonString)
                                              .Items
                                              .Single()
                                              .Statistics
                                              .ViewCount;

                if (viewCount.HasValue)
                {
                    Views = viewCount.Value;
                }
            }
        }
    }
}