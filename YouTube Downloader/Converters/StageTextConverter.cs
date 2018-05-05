namespace YouTube.Downloader.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    using Caliburn.Micro;

    using YouTube.Downloader.Core;
    using YouTube.Downloader.Models;
    using YouTube.Downloader.Models.Download;
    using YouTube.Downloader.Services.Interfaces;

    [ValueConversion(typeof(int), typeof(string))]
    internal class StageTextConverter : IValueConverter
    {
        private static readonly Settings Settings = IoC.Get<ISettingsService>().Settings;

        public static StageTextConverter Default { get; } = new StageTextConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int stage = (int)value;

            if (stage == 0)
            {
                return "Gathering Data";
            }

            if (Settings.DownloadType == DownloadType.Audio)
            {
                switch (stage)
                {
                    case 1:
                        return "Downloading Audio";

                    case 2:
                        return "Finalising";

                    default:
                        return "Unknown";
                }
            }

            switch (stage)
            {
                case 1:
                    return "Downloading Video";

                case 2:
                    return "Downloading Audio";

                case 3:
                    return "Finalising";

                default:
                    return "Unknown";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw ThrowHelper.NotSupported();
        }
    }
}