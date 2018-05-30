namespace YouTube.Downloader.Core.Downloading
{
    using System.IO;

    using YouTube.Downloader.Models.Download;

    internal class ConvertProcess : MonitoredProcess
    {
        private readonly string _filename;

        internal ConvertProcess(string filename, string newExtension, DownloadStatus downloadStatus) : base("ffmpeg", $"-i \"{filename}\" \"{Path.ChangeExtension(filename, newExtension)}\"", downloadStatus)
        {
            _filename = filename;
        }

        private protected override void OnExited(bool killed)
        {
            if (!killed)
            {
                File.Delete(_filename);
            }
        }
    }
}