namespace YouTube.Downloader.Models.Download
{
    internal enum DownloadState
    {
        Queued,
        Downloading,
        Paused,
        Completed,
        Exited
    }
}