namespace YouTube.Downloader.Services
{
    using System.IO;
    using System.Threading;

    using Google.Apis.Auth.OAuth2;
    using Google.Apis.Services;
    using Google.Apis.Util.Store;

    using YouTube.Downloader.Services.Interfaces;

    using YouTubeApiService = Google.Apis.YouTube.v3.YouTubeService;

    internal class YouTubeService : IYouTubeService
    {
        private readonly YouTubeApiService _youTubeApiService;

        public YouTubeService()
        {
            using (FileStream clientSecrets = File.OpenRead("Client Secrets.json"))
            {
                _youTubeApiService = new YouTubeApiService(new BaseClientService.Initializer
                {
                        HttpClientInitializer = GoogleWebAuthorizationBroker.AuthorizeAsync(GoogleClientSecrets.Load(clientSecrets).Secrets,
                                                                                            new string[] { YouTubeApiService.Scope.YoutubeReadonly },
                                                                                            "user",
                                                                                            CancellationToken.None,
                                                                                            new FileDataStore("YouTube Downloader")).Result
                });
            }
        }
    }
}