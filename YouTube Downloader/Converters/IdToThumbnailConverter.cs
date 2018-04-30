namespace YouTube.Downloader.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    using YouTube.Downloader.Core;

    [ValueConversion(typeof(string), typeof(string))]
    internal class IdToThumbnailConverter : IValueConverter
    {
        public static IdToThumbnailConverter Default { get; } = new IdToThumbnailConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return $"https://img.youtube.com/vi/{(string)value}/hqdefault.jpg";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw ThrowHelper.NotSupported();
        }
    }
}