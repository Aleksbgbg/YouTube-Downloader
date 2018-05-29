namespace YouTube.Downloader.Core.Downloading
{
    using System.IO;

    internal class ConvertProcess : MonitoredProcess
    {
        internal ConvertProcess(string filename, string newExtension) : base("ffmpeg", $"-i \"{filename}\" \"{Path.ChangeExtension(filename, newExtension)}\"")
        {
        }
    }
}