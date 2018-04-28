namespace YouTube.Downloader.Helpers
{
    using System;
    using System.Diagnostics;

    using YouTube.Downloader.Models;

    internal class Download
    {
        private readonly string _processArguments;

        private bool _isPaused;

        private bool _isExited;

        internal Download(YouTubeVideo video, Settings settings)
        {
            _processArguments = $"-o {settings.DownloadPath}/%(title)s.%(ext)s" + " " +
                                $"-f {(settings.DownloadType == DownloadType.Audio ? "bestaudio" : "bestvideo+bestaudio")}" + " " +
                                $"\"{video.Id}\"";
        }

        internal event EventHandler Completed;

        internal event EventHandler Paused;

        internal event EventHandler Resumed;

        internal event EventHandler Killed;

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

                    _isExited = true;

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
            if (_isPaused)
            {
                throw new InvalidOperationException("Cannot pause a paused download.");
            }

            KillProcess(Paused);
            _isPaused = true;
        }

        internal void Resume()
        {
            if (!_isPaused)
            {
                throw new InvalidOperationException("Cannot resume a download in progress.");
            }

            GenerateProcess();
            Process.Start();

            _isPaused = false;

            Resumed?.Invoke(this, EventArgs.Empty);
        }

        internal void Kill()
        {
            KillProcess(Killed);
        }

        private void KillProcess(EventHandler invokeEvent)
        {
            if (!_isExited && !_isPaused)
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