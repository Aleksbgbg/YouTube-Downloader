namespace YouTube.Downloader.Core.Downloading
{
    using System.Text.RegularExpressions;

    internal static class ProgressMonitoringRegexes
    {
        internal static Regex ProgressRegex { get; } = new Regex(@"^\[download] (?<ProgressPercentage>[ 1][ 0-9][0-9]\.[0-9])% of .*?(?<TotalDownloadSize>[\d\.]+)?(?<TotalDownloadSizeUnits>.iB) at +(?:(?<DownloadSpeed>.+)(?<DownloadSpeedUnits>.iB)\/s|Unknown speed)");

        internal static Regex DestinationRegex { get; } = new Regex(@"^\[download] Destination: (?<Filename>.+)$");
    }
}