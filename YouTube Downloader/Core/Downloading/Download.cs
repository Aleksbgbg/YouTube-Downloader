namespace YouTube.Downloader.Core.Downloading
{
    using System;
    using System.Diagnostics;

    using YouTube.Downloader.Models;
    using YouTube.Downloader.Models.Download;

    internal class Download
    {
        private readonly string _processArguments;

        private bool _isPaused;

        internal Download(YouTubeVideo video, DownloadStatus downloadStatus, Settings settings)
        {
            YouTubeVideo = video;
            DownloadStatus = downloadStatus;

            _processArguments = $"-o {settings.DownloadPath}/%(title)s.%(ext)s -f {(settings.DownloadType == DownloadType.Audio ? "bestaudio" : "bestvideo+bestaudio")} \"{video.Id}\"";
        }

        internal event EventHandler Exited;

        public YouTubeVideo YouTubeVideo { get; }

        public DownloadStatus DownloadStatus { get; }

        private Process _process;
        public Process Process
        {
            get => _process;

            private set
            {
                void ProcessExited(object sender, EventArgs e)
                {
                    Process.Exited -= ProcessExited;
                    Process = null;

                    if (_isPaused) return;

                    Exited?.Invoke(this, EventArgs.Empty);

                    if (DownloadStatus.DownloadState == DownloadState.Exited) return;

                    DownloadStatus.DownloadState = DownloadState.Completed;
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
            GenerateAndStartProcess();
            DownloadStatus.DownloadState = DownloadState.Downloading;
        }

        internal void Pause()
        {
            if (_isPaused)
            {
                throw new InvalidOperationException("Cannot pause a paused download.");
            }

            Process.Kill();
            _isPaused = true;

            DownloadStatus.DownloadState = DownloadState.Paused;
        }

        internal void Resume()
        {
            if (!_isPaused)
            {
                throw new InvalidOperationException("Cannot resume a download in progress.");
            }

            GenerateAndStartProcess();

            _isPaused = false;

            DownloadStatus.DownloadState = DownloadState.Downloading;
        }

        internal void Kill()
        {
            if (Process == null)
            {
                Exited?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                Process.Kill();
            }

            DownloadStatus.DownloadState = DownloadState.Exited;
        }

        internal void TogglePause()
        {
            if (_isPaused)
            {
                Resume();
                return;
            }

            Pause();
        }

        private void GenerateAndStartProcess()
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
            Process.Start();

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
            progressMonitor.Run();
        }
    }
}