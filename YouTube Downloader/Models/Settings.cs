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
    }
}