namespace YouTube.Downloader.Models.Download
{
    internal enum DownloadState
    {
        Queued,
        Paused,
        Downloading,
        Converting,
        Completed,
        Exited
    }
}