namespace YouTube.Downloader.Services.Interfaces
{
    using System.Collections.Generic;

    using YouTube.Downloader.Core.Downloading;

    internal interface IDownloadService
    {
        IEnumerable<Download> Downloads { get; }

        void QueueDownloads(IEnumerable<Download> downloads);

        void ResumeDownloads(IEnumerable<Download> downloads);
    }
}