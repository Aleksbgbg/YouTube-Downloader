namespace YouTube.Downloader.Services
{
    using System.IO;
    using System.Threading;

    using Google.Apis.Auth.OAuth2;
    using Google.Apis.Services;
    using Google.Apis.Util.Store;
    using Google.Apis.YouTube.v3;

    using YouTube.Downloader.Services.Interfaces;

    internal class YouTubeApiService : IYouTubeApiService
    {
        private readonly YouTubeService _youTubeApiService;

        public YouTubeApiService()
        {
            using (FileStream clientSecrets = File.OpenRead("Client Secret.json"))
            {
                _youTubeApiService = new YouTubeService(new BaseClientService.Initializer
                {
                        HttpClientInitializer = GoogleWebAuthorizationBroker.AuthorizeAsync(GoogleClientSecrets.Load(clientSecrets).Secrets,
                                                                                            new string[] { YouTubeService.Scope.YoutubeReadonly },
                                                                                            "user",
                                                                                            CancellationToken.None,
                                                                                            new FileDataStore("YouTube Downloader")).Result
                });
            }
        }
    }
}