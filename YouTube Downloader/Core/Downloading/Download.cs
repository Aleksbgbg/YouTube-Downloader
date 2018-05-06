namespace YouTube.Downloader.Core.Downloading
{
    using System;
    using System.Diagnostics;

    using YouTube.Downloader.Models;
    using YouTube.Downloader.Models.Download;

    internal class Download
    {
        private readonly string _processArguments;

        private readonly DownloadStatus _downloadStatus;

        private bool _isPaused;

        internal Download(YouTubeVideo video, DownloadStatus downloadStatus, Settings settings)
        {
            YouTubeVideo = video;
            _downloadStatus = downloadStatus;

            _processArguments = $"-o \"{settings.DownloadPath}\\%(title)s.%(ext)s\" -f {(settings.DownloadType == DownloadType.Audio ? "bestaudio" : "bestvideo+bestaudio")} -- \"{video.Id}\"";
        }

        internal event EventHandler Exited;

        internal bool HasExited => _downloadStatus.DownloadState == DownloadState.Exited;

        internal bool CanPause => !_isPaused && Process != null;

        internal bool CanResume => _isPaused;

        internal bool CanKill => _downloadStatus.DownloadState == DownloadState.Downloading ||
                                 _downloadStatus.DownloadState == DownloadState.Paused ||
                                 _downloadStatus.DownloadState == DownloadState.Queued;

        internal YouTubeVideo YouTubeVideo { get; }

        private Process _process;
        internal Process Process
        {
            get => _process;

            private set
            {
                void ProcessExited(object sender, EventArgs e)
                {
                    Process.Exited -= ProcessExited;
                    Process = null;

                    if (_isPaused) return;

                    if (_downloadStatus.DownloadState != DownloadState.Exited)
                    {
                        _downloadStatus.DownloadState = DownloadState.Completed;
                    }

                    OnExited();
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
            _downloadStatus.DownloadState = DownloadState.Downloading;
        }

        internal void Pause()
        {
            if (_isPaused)
            {
                throw new InvalidOperationException("Cannot pause a paused download.");
            }

            Process.Kill();
            _isPaused = true;

            _downloadStatus.DownloadState = DownloadState.Paused;
        }

        internal void Resume()
        {
            if (!_isPaused)
            {
                throw new InvalidOperationException("Cannot resume a download in progress.");
            }

            GenerateAndStartProcess();

            _isPaused = false;

            _downloadStatus.DownloadState = DownloadState.Downloading;
        }

        internal void Kill()
        {
            _downloadStatus.DownloadState = DownloadState.Exited;

            if (Process == null)
            {
                OnExited();
            }
            else
            {
                try
                {
                    Process.Kill();
                }
                catch (InvalidOperationException)
                {
                    OnExited();
                }
            }
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
                _downloadStatus.DownloadProgress.Stage = e.Stage;

                if (e.DownloadSpeed.HasValue)
                {
                    _downloadStatus.DownloadProgress.DownloadSpeed = e.DownloadSpeed.Value;
                }

                _downloadStatus.DownloadProgress.ProgressPercentage = e.DownloadPercentage;
                _downloadStatus.DownloadProgress.TotalDownloadSize = e.TotalDownloadSize;
            });
            progressMonitor.Run();
        }

        private void OnExited()
        {
            Exited?.Invoke(this, EventArgs.Empty);
        }
    }
}