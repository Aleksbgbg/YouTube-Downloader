namespace YouTube.Downloader.Models
{
    using System;
    using System.ComponentModel;

    using Caliburn.Micro;

    using Newtonsoft.Json;

    using YouTube.Downloader.Controls;
    using YouTube.Downloader.Models.Download;

    internal class Settings : PropertyChangedBase
    {
        private string _downloadPath;
        [DisplayName("Download Path")]
        [Category("Download")]
        [Description("Path where downloaded videos are saved.")]
        [Editor(typeof(DownloadPathControl), typeof(DownloadPathControl))]
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
        [Category("Download")]
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

                if (OutputFormat == OutputFormat.Mp3 && _downloadType == DownloadType.AudioVideo ||
                    OutputFormat == OutputFormat.Mp4 && _downloadType == DownloadType.Audio)
                {
                    OutputFormat = OutputFormat.Auto;
                }
            }
        }

        private OutputFormat _outputFormat;
        [DisplayName("Output Format")]
        [Category("Output")]
        [Description("Select the output container for downloaded videos.")]
        [JsonProperty("OutputFormat")]
        public OutputFormat OutputFormat
        {
            get => _outputFormat;

            set
            {
                if (_outputFormat == value) return;

                _outputFormat = value;
                NotifyOfPropertyChange(() => OutputFormat);

                if (_outputFormat == OutputFormat.Mp3)
                {
                    DownloadType = DownloadType.Audio;
                }
                else if (_outputFormat == OutputFormat.Mp4)
                {
                    DownloadType = DownloadType.AudioVideo;
                }
            }
        }
    }
}