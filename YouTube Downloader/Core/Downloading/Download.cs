namespace YouTube.Downloader.Core.Downloading
{
    using System;
    using System.Diagnostics;

    using YouTube.Downloader.Models;
    using YouTube.Downloader.Models.Download;

    internal class Download
    {
        private readonly string _processArguments;

        internal Download(YouTubeVideo video, Settings settings)
        {
            YouTubeVideo = video;

            _processArguments = $"-o {settings.DownloadPath}/%(title)s.%(ext)s" + " " +
                                $"-f {(settings.DownloadType == DownloadType.Audio ? "bestaudio" : "bestvideo+bestaudio")}" + " " +
                                $"\"{video.Id}\"";
        }

        internal event EventHandler Completed;

        internal event EventHandler Paused;

        internal event EventHandler Resumed;

        internal event EventHandler Killed;

        public bool IsPaused { get; private set; }

        public bool IsExited { get; private set; }

        public YouTubeVideo YouTubeVideo { get; }

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

            private set
            {
                void ProcessExited(object sender, EventArgs e)
                {
                    _process.Exited -= ProcessExited;

                    IsExited = true;

                    if (_process != null)
                    {
                        Completed?.Invoke(this, EventArgs.Empty);
                    }
                }

                if (_process != null)
                {
                    _process.Exited -= ProcessExited;
                }

                _process = value;

                if (_process != null)
                {
                    _process.Exited += ProcessExited;
                }
            }
        }

        internal void Start()
        {
            Process.Start();
        }

        internal void Pause()
        {
            if (IsPaused)
            {
                throw new InvalidOperationException("Cannot pause a paused download.");
            }

            KillProcess(Paused);
            IsPaused = true;
        }

        internal void Resume()
        {
            if (!IsPaused)
            {
                throw new InvalidOperationException("Cannot resume a download in progress.");
            }

            GenerateProcess();
            Process.Start();

            IsPaused = false;

            Resumed?.Invoke(this, EventArgs.Empty);
        }

        internal void TogglePause()
        {
            if (IsPaused)
            {
                Resume();
                return;
            }

            Pause();
        }

        internal void Kill()
        {
            KillProcess(Killed);
        }

        private void KillProcess(EventHandler invokeEvent)
        {
            if (!IsExited && !IsPaused)
            {
                Process.Kill();
            }

            Process = null;

            invokeEvent?.Invoke(this, EventArgs.Empty);
        }

        private void GenerateProcess()
        {
            Process = new Process
            {
                EnableRaisingEvents = true,
                StartInfo = new ProcessStartInfo("Resources/youtube-dl.exe", _processArguments)
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true
                }
            };
        }
    }
}