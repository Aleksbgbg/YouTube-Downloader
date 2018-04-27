namespace YouTube.Downloader.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    [ValueConversion(typeof(long), typeof(string))]
    internal class AppropriateDataSizeConverter : IValueConverter
    {
        private static readonly char[] StoragePrefixes = { 'K', 'M', 'G', 'T', 'P', 'E' };

        public static AppropriateDataSizeConverter Default { get; } = new AppropriateDataSizeConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double bytes = (long)value;

            int loopCount = 0;

            while (bytes >= 1024)
            {
                bytes /= 1024;
                ++loopCount;
            }

            return loopCount == 0 ? $"{bytes:N0} B" : $"{bytes:N2} {StoragePrefixes[loopCount - 1]}iB";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("ConvertBack is not supported.");
        }
    }
}