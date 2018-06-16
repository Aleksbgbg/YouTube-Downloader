namespace YouTube.Downloader.Utilities.Downloading
{
    internal class Progress
    {
        internal Progress(long totalDownloadSize, double downloadPercentage, long? downloadSpeed, int stage)
        {
            TotalDownloadSize = totalDownloadSize;
            DownloadPercentage = downloadPercentage;
            DownloadSpeed = downloadSpeed;
            Stage = stage;
        }

        internal long TotalDownloadSize { get; }

        internal double DownloadPercentage { get; }

        internal long? DownloadSpeed { get; }

        internal int Stage { get; }
    }
}