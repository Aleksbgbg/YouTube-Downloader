namespace YouTube.Downloader.Utilities.Processing
{
    using System.Text;

    using YouTube.Downloader.Models;
    using YouTube.Downloader.Models.Download;

    internal class DownloadStartInfo
    {
        internal string DownloadFolder { get; set; }

        internal DownloadType DownloadType { get; set; }

        internal OutputFormat OutputFormat { get; set; }

        internal string VideoName { get; set; }

        internal string VideoId { get; set; }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendFormat("-o \"{0}\\{1}.%(ext)s\"", DownloadFolder, VideoName);

            stringBuilder.Append(" -f ");

            if (DownloadType == DownloadType.AudioVideo)
            {
                if (OutputFormat == OutputFormat.Mp4)
                {
                    stringBuilder.AppendFormat("bestvideo[ext=mp4]+bestaudio[ext=m4a]/best[ext=mp4]/best");
                }
                else
                {
                    stringBuilder.Append("bestvideo+bestaudio/best");
                }
            }
            else
            {
                stringBuilder.Append("bestaudio");

                if (OutputFormat == OutputFormat.Mp3)
                {
                    stringBuilder.AppendFormat("[ext=mp3]");
                }

                stringBuilder.Append("/best");
            }

            stringBuilder.AppendFormat(" -- \"{0}\"", VideoId);

            return stringBuilder.ToString();
        }
    }
}