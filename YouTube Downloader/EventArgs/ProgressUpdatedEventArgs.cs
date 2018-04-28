namespace YouTube.Downloader.EventArgs
{
    using System;

    internal class ProgressUpdatedEventArgs : EventArgs
    {
        internal ProgressUpdatedEventArgs(long totalDownloadSize, double downloadPercentage, long? downloadSpeed, int stage)
        {
            TotalDownloadSize = totalDownloadSize;
            DownloadPercentage = downloadPercentage;
            DownloadSpeed = downloadSpeed;
            Stage = stage;
        }

        public long TotalDownloadSize { get; }

        public double DownloadPercentage { get; }

        public long? DownloadSpeed { get; }

        public int Stage { get; }
    }
}