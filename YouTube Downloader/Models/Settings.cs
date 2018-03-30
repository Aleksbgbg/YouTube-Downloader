namespace YouTube.Downloader.Models
{
    using System.ComponentModel;

    internal class Settings
    {
        [DisplayName("Download Path")]
        [Description("Path where downloaded videos are saved.")]
        public string DownloadPath { get; set; }
    }
}