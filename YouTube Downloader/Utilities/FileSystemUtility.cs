namespace YouTube.Downloader.Utilities
{
    using System;
    using System.Runtime.InteropServices;

    using YouTube.Downloader.Utilities.Interfaces;

    internal class FileSystemUtility : IFileSystemUtility
    {
        private static readonly Guid DownloadsFolder = new Guid("374DE290-123F-4565-9164-39C4925E467B");

        public string DownloadsFolderPath
        {
            get
            {
                SHGetKnownFolderPath(DownloadsFolder, 0, IntPtr.Zero, out string downloadsFolderPath);

                return downloadsFolderPath;
            }
        }

        [DllImport("Shell32.dll", CharSet = CharSet.Unicode)]
        private static extern int SHGetKnownFolderPath([MarshalAs(UnmanagedType.LPStruct)] Guid guid, uint flags, IntPtr tokenHandle, out string path);
    }
}