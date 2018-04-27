namespace YouTube.Downloader.Helpers
{
    using System.Collections.Generic;
    using System.Diagnostics;

    using YouTube.Downloader.Models;

    internal class Download
    {
        private readonly List<string> _processArguments;

        internal Download(YouTubeVideo video, Settings settings)
        {
            _processArguments = new List<string>
            {
                $"-o {settings.DownloadPath}/%(title)s.%(ext)s",
                $"-f {(settings.DownloadType == DownloadType.Audio ? "bestaudio" : "bestvideo+bestaudio")}",
                $"\"{video.Id}\""
            };
        }

        private Process _process;
        public Process Process
        {
            get
            {
                if (_process == null)
                {
                    GenerateProcess();
                }

                return _process;
            }

            private set => _process = value;
        }

        internal void Start()
        {
            if (_processArguments.Contains("-c"))
            {
                _processArguments.Remove("-c");
            }

            Process.Start();
        }

        internal void Pause()
        {
            Kill();
            Process = null;
        }

        internal void Resume()
        {
            _processArguments.Add("-c");
            GenerateProcess();
            Process.Start();
        }

        internal void Kill()
        {
            Process.Kill();
        }

        private void GenerateProcess()
        {
            Process = new Process
            {
                EnableRaisingEvents = true,
                StartInfo = new ProcessStartInfo("Resources/youtube-dl.exe", string.Join(" ", _processArguments))
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true
                }
            };
        }
    }
}