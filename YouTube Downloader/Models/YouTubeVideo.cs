namespace YouTube.Downloader.Models
{
    internal class YouTubeVideo
    {
        internal YouTubeVideo(string id, string title)
        {
            Id = id;
            Title = title;
        }

        internal string Id { get; }

        public string Title { get; }
    }
}