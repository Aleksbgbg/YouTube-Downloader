namespace YouTube.Downloader.Core.Downloading
{
    using System.IO;

    using YouTube.Downloader.Models.Download;

    internal class ConvertProcess : MonitoredProcess
    {
        internal ConvertProcess(string filename, string newExtension, DownloadStatus downloadStatus) : base("ffmpeg", $"-i \"{filename}\" \"{Path.ChangeExtension(filename, newExtension)}\"", downloadStatus)
        {
        }
    }
}