namespace YouTube.Downloader.Models
{
    using System.ComponentModel;

    using YouTube.Downloader.Utilities;

    [TypeConverter(typeof(EnumDescriptionConverter))]
    internal enum OutputFormat
    {
        Auto,
        [Description("MP4")]
        Mp4,
        [Description("MP3")]
        Mp3
    }
}