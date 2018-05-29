namespace YouTube.Downloader.Models
{
    using Newtonsoft.Json;

    using YouTube.Downloader.Extensions;

    internal class QueryResult
    {
        internal QueryResult(string query)
        {
            Query = query;
        }

        [JsonConstructor]
        internal QueryResult(string query, YouTubeVideo matchedVideo) : this(query)
        {
            MatchedVideo = matchedVideo;
        }

        internal QueryResult(QueryResult queryResult, YouTubeVideo matchedVideo) : this(queryResult.Query, matchedVideo)
        {
        }

        [JsonProperty("Query")]
        public string Query { get; }

        private YouTubeVideo _matchedVideo;
        [JsonProperty("MatchedVideo")]
        public YouTubeVideo MatchedVideo
        {
            get => _matchedVideo;

            set
            {
                _matchedVideo = value;
                PercentageMatch = Query.PercentageSimilarity(_matchedVideo.Title);
            }
        }

        [JsonIgnore]
        public double PercentageMatch { get; private set; }
    }
}