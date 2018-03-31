namespace YouTube.Downloader.Models
{
    using System.ComponentModel;

    using YouTube.Downloader.Helpers;

    [TypeConverter(typeof(EnumDescriptionConverter))]
    internal enum DownloadType
    {
        [Description("Audio and Video")]
        AudioVideo,
        [Description("Audio Only")]
        Audio
    }
}