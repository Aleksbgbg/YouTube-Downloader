namespace YouTube.Downloader.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    using YouTube.Downloader.Utilities;

    [ValueConversion(typeof(int), typeof(bool))]
    internal class CountToBooleanConverter : IValueConverter
    {
        public static CountToBooleanConverter Default { get; } = new CountToBooleanConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)value > 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw ThrowHelper.NotSupported();
        }
    }
}