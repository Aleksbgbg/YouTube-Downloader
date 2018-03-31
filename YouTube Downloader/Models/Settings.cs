namespace YouTube.Downloader.Models
{
    using System.ComponentModel;

    using Caliburn.Micro;

    using Newtonsoft.Json;

    using YouTube.Downloader.Views;

    internal class Settings : PropertyChangedBase
    {
        private string _downloadPath;
        [DisplayName("Download Path")]
        [Description("Path where downloaded videos are saved.")]
        [Editor(typeof(DownloadPathView), typeof(DownloadPathView))]
        [JsonProperty("DownloadPath")]
        public string DownloadPath
        {
            get => _downloadPath;

            set
            {
                if (_downloadPath == value) return;

                _downloadPath = value;
                NotifyOfPropertyChange(() => DownloadPath);
            }
        }

        private DownloadType _downloadType;
        [DisplayName("Download Type")]
        [Description("Choose whether to download just audio, or audio and video.")]
        [JsonProperty("DownloadType")]
        public DownloadType DownloadType
        {
            get => _downloadType;

            set
            {
                if (_downloadType == value) return;

                _downloadType = value;
                NotifyOfPropertyChange(() => DownloadType);
            }
        }
    }
}