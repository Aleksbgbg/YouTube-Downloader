namespace YouTube.Downloader.Core.Downloading
{
    using System;

    using YouTube.Downloader.EventArgs;
    using YouTube.Downloader.Models;
    using YouTube.Downloader.Models.Download;

    internal class Download
    {
        private readonly string _processArguments;

        private readonly DownloadStatus _downloadStatus;

        private bool _isRequeued;

        internal Download(DownloadStatus downloadStatus, Settings settings, YouTubeVideo youTubeVideo)
        {
            _downloadStatus = downloadStatus;
            _processArguments = $"-o \"{settings.DownloadPath}\\%(title)s.%(ext)s\" -f {(settings.DownloadType == DownloadType.Audio ? "bestaudio" : "bestvideo+bestaudio")} -- \"{youTubeVideo.Id}\"";
            YouTubeVideo = youTubeVideo;
        }

        internal event EventHandler<DownloadFinishedEventArgs> Finished;

        internal event EventHandler Paused;

        internal event EventHandler Resumed;

        internal YouTubeVideo YouTubeVideo { get; }

        internal bool CanPause => HasStarted && !HasExited && !IsPaused;

        internal bool CanResume => HasStarted && !HasExited && IsPaused && !_isRequeued;

        internal bool CanKill => !HasExited;

        private bool _hasStarted;
        internal bool HasStarted
        {
            get => _hasStarted;

            private set
            {
                _hasStarted = value;
                UpdateDownloadState();
            }
        }

        private bool _isPaused;
        internal bool IsPaused
        {
            get => _isPaused;

            private set
            {
                _isPaused = value;
                UpdateDownloadState();
            }
        }

        private bool _hasExited;
        internal bool HasExited
        {
            get => _hasExited;

            private set
            {
                _hasExited = value;
                UpdateDownloadState();

                if (_hasExited)
                {
                    Finished?.Invoke(this, new DownloadFinishedEventArgs(DidComplete));
                }
            }
        }

        private bool _didComplete;
        internal bool DidComplete
        {
            get => _didComplete;

            private set
            {
                _didComplete = value;
                UpdateDownloadState();
            }
        }

        private DownloadProcess _downloadProcess;
        private DownloadProcess DownloadProcess
        {
            get => _downloadProcess;

            set
            {
                void DownloadProcessExited(object sender, EventArgs e)
                {
                    _downloadProcess.Exited -= DownloadProcessExited;

                    if (IsPaused || HasExited)
                    {
                        return;
                    }

                    DidComplete = true;
                    HasExited = true;
                }

                if (_downloadProcess != null)
                {
                    _downloadProcess.Exited -= DownloadProcessExited;
                }

                _downloadProcess = value;

                if (_downloadProcess == null)
                {
                    return;
                }

                _downloadProcess.Exited += DownloadProcessExited;

                _downloadProcess.ProgressMonitor.MonitorDownload((sender, e) =>
                {
                    _downloadStatus.DownloadProgress.Stage = e.Stage;

                    if (e.DownloadSpeed.HasValue)
                    {
                        _downloadStatus.DownloadProgress.DownloadSpeed = e.DownloadSpeed.Value;
                    }

                    _downloadStatus.DownloadProgress.ProgressPercentage = e.DownloadPercentage;
                    _downloadStatus.DownloadProgress.TotalDownloadSize = e.TotalDownloadSize;
                });
            }
        }

        internal void Start()
        {
            if (HasStarted)
            {
                throw new InvalidOperationException("Cannot start an already started download.");
            }

            HasStarted = true;
            GenerateAndStartProcess();
        }

        internal void Kill()
        {
            if (!HasStarted || IsPaused)
            {
                HasExited = true;
                return;
            }

            if (HasExited)
            {
                throw new InvalidOperationException("Cannot kill a download which has already been killed.");
            }

            HasExited = true;
            KillProcess();
        }

        internal void Pause()
        {
            if (!HasStarted)
            {
                throw new InvalidOperationException("Cannot pause a download which has not been started.");
            }

            if (HasExited)
            {
                throw new InvalidOperationException("Cannot pause a download which has already been killed.");
            }

            if (IsPaused)
            {
                throw new InvalidOperationException("Cannot pause a paused download.");
            }

            IsPaused = true;
            KillProcess();
            Paused?.Invoke(this, EventArgs.Empty);
        }

        internal void Resume()
        {
            if (!IsPaused)
            {
                throw new InvalidOperationException("Cannot resume a download which has not been paused.");
            }

            _isRequeued = false;
            IsPaused = false;
            GenerateAndStartProcess();
            Resumed?.Invoke(this, EventArgs.Empty);
        }

        internal void OnRequeued()
        {
            _downloadStatus.DownloadState = DownloadState.Queued;
            _isRequeued = true;
        }

        private void GenerateAndStartProcess()
        {
            DownloadProcess = new DownloadProcess(_processArguments);
            DownloadProcess.Start();
        }

        private void KillProcess()
        {
            DownloadProcess.Kill();
            DownloadProcess = null;
        }

        private void UpdateDownloadState()
        {
            if (HasExited)
            {
                _downloadStatus.DownloadState = DidComplete ? DownloadState.Completed : DownloadState.Exited;
            }
            else if (IsPaused)
            {
                _downloadStatus.DownloadState = DownloadState.Paused;
            }
            else if (HasStarted)
            {
                _downloadStatus.DownloadState = DownloadState.Downloading;
            }
            else
            {
                _downloadStatus.DownloadState = DownloadState.Queued;
            }
        }
    }
}