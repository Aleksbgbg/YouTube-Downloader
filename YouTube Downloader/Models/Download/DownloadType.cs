namespace YouTube.Downloader.Models.Download
{
    using System.ComponentModel;

    using YouTube.Downloader.Core;

    [TypeConverter(typeof(EnumDescriptionConverter))]
    internal enum DownloadType
    {
        [Description("Audio and Video")]
        AudioVideo,
        [Description("Audio Only")]
        Audio
    }
}