namespace YouTube.Downloader.Utilities
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Threading.Tasks;

    using YouTube.Downloader.Extensions;

    internal static class YouTubeDl
    {
        internal static async Task Update()
        {
            await new Process
            {
                StartInfo = new ProcessStartInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources/youtube-dl.exe"), "-U")
                {
                    CreateNoWindow = true,
                    UseShellExecute = false
                }
            }.RunAndWaitForExitAsync();
        }
    }
}