namespace YouTube.Downloader.Converters
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Windows.Data;
    using System.Windows.Media;

    using YouTube.Downloader.Core;

    [ValueConversion(typeof(double), typeof(SolidColorBrush))]
    internal class PercentageMatchColourConverter : IValueConverter
    {
        private static readonly GradientStopCollection GradientStops = new GradientStopCollection(new GradientStop[]
        {
                new GradientStop(Color.FromRgb(255, 0, 0), 0),
                new GradientStop(Color.FromRgb(199, 0, 52), 0.5),
                new GradientStop(Color.FromRgb(57, 181, 74), 1)
        });

        public static PercentageMatchColourConverter Default { get; } = new PercentageMatchColourConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double percentage = (double)value;

            GradientStop start = GradientStops.First();
            GradientStop stop = GradientStops.Last();

            foreach (GradientStop gradientStop in GradientStops)
            {
                if (start.Offset < gradientStop.Offset && gradientStop.Offset < percentage)
                {
                    start = gradientStop;
                }

                if (percentage < gradientStop.Offset && gradientStop.Offset < stop.Offset)
                {
                    stop = gradientStop;
                }
            }

            return new SolidColorBrush(Color.FromScRgb((float)((percentage - stop.Offset) * (start.Color.ScA - stop.Color.ScA) / (start.Offset - stop.Offset) + stop.Color.ScA),
                                                       (float)((percentage - stop.Offset) * (start.Color.ScR - stop.Color.ScR) / (start.Offset - stop.Offset) + stop.Color.ScR),
                                                       (float)((percentage - stop.Offset) * (start.Color.ScG - stop.Color.ScG) / (start.Offset - stop.Offset) + stop.Color.ScG),
                                                       (float)((percentage - stop.Offset) * (start.Color.ScB - stop.Color.ScB) / (start.Offset - stop.Offset) + stop.Color.ScB)));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw ThrowHelper.NotSupported();
        }
    }
}