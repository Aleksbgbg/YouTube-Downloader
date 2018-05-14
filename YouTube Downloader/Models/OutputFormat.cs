namespace YouTube.Downloader.Models
{
    using System.ComponentModel;

    using YouTube.Downloader.Core;

    [TypeConverter(typeof(EnumDescriptionConverter))]
    internal enum OutputFormat
    {
        [Description("MP3")]
        Mp3,
        [Description("MP4")]
        Mp4,
        Auto
    }
}