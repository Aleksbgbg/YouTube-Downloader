namespace YouTube.Downloader.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using YouTube.Downloader.Core;

    [ValueConversion(typeof(bool), typeof(string))]
    internal class TogglePauseTextConverter : IMultiValueConverter
    {
        public static TogglePauseTextConverter Default { get; } = new TogglePauseTextConverter();

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            bool canPause = (bool)values[0];
            bool canResume = (bool)values[1];

            if (canPause)
            {
                return "Pause";
            }

            if (canResume)
            {
                return "Resume";
            }

            return "Toggle Pause";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw ThrowHelper.NotSupported();
        }
    }
}