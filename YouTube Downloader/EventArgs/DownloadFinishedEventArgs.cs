namespace YouTube.Downloader.EventArgs
{
    using System;

    internal class DownloadFinishedEventArgs : EventArgs
    {
        internal DownloadFinishedEventArgs(bool didComplete)
        {
            DidComplete = didComplete;
        }

        internal bool DidComplete { get; }
    }
}