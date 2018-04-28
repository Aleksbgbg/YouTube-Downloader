namespace YouTube.Downloader.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    using YouTube.Downloader.Models;

    internal class Download
    {
        private const string ContinueSwitch = "-c";

        private readonly List<string> _processArguments;

        private bool _isPaused;

        private bool _isExited;

        internal Download(YouTubeVideo video, Settings settings)
        {
            _processArguments = new List<string>
            {
                $"-o {settings.DownloadPath}/%(title)s.%(ext)s",
                $"-f {(settings.DownloadType == DownloadType.Audio ? "bestaudio" : "bestvideo+bestaudio")}",
                $"\"{video.Id}\""
            };
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
            if (_processArguments.Contains(ContinueSwitch))
            {
                _processArguments.Remove(ContinueSwitch);
            }

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

            _processArguments.Add(ContinueSwitch);
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
            if (!_isExited)
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