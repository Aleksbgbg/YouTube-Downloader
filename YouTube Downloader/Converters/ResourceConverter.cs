namespace YouTube.Downloader.Converters
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    using YouTube.Downloader.Core;

    [ValueConversion(typeof(string), typeof(object))]
    internal class ResourceConverter : IValueConverter
    {
        public static ResourceConverter Default { get; } = new ResourceConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Application.Current.FindResource((string)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw ThrowHelper.NotSupported();
        }
    }
}