namespace YouTube.Downloader.Core.Downloading
{
    using System;
    using System.Diagnostics;

    using YouTube.Downloader.Models;
    using YouTube.Downloader.Models.Download;

    internal class Download
    {
        private readonly string _processArguments;

        internal Download(YouTubeVideo video, DownloadStatus downloadStatus, Settings settings)
        {
            YouTubeVideo = video;
            DownloadStatus = downloadStatus;

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

        public DownloadStatus DownloadStatus { get; }

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

                    DownloadStatus.DownloadState = DownloadState.Completed;

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
            ProgressMonitor progressMonitor = new ProgressMonitor(Process);

            progressMonitor.MonitorDownload((sender, e) =>
            {
                DownloadStatus.DownloadProgress.Stage = e.Stage;

                if (e.DownloadSpeed.HasValue)
                {
                    DownloadStatus.DownloadProgress.DownloadSpeed = e.DownloadSpeed.Value;
                }

                DownloadStatus.DownloadProgress.ProgressPercentage = e.DownloadPercentage;
                DownloadStatus.DownloadProgress.TotalDownloadSize = e.TotalDownloadSize;
            });

            Process.Start();

            progressMonitor.Run();

            DownloadStatus.DownloadState = DownloadState.Downloading;
        }

        internal void Pause()
        {
            if (IsPaused)
            {
                throw new InvalidOperationException("Cannot pause a paused ");
            }

            KillProcess(Paused);
            IsPaused = true;

            DownloadStatus.DownloadState = DownloadState.Paused;
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

            DownloadStatus.DownloadState = DownloadState.Downloading;
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
            DownloadStatus.DownloadState = DownloadState.Exited;
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