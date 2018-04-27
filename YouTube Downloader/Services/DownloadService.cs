namespace YouTube.Downloader.Services
{
    using System;
    using System.Collections.Generic;

    using Caliburn.Micro;

    using YouTube.Downloader.EventArgs;
    using YouTube.Downloader.Helpers;
    using YouTube.Downloader.Models;
    using YouTube.Downloader.Services.Interfaces;
    using YouTube.Downloader.ViewModels.Interfaces;

    internal class DownloadService : IDownloadService
    {
        private const int MaxConcurrentDownloads = 3;

        private readonly Queue<IDownloadViewModel> _downloadQueue = new Queue<IDownloadViewModel>();

        private readonly List<Download> _currentDownloads = new List<Download>();

        private readonly ISettingsService _settingsService;

        public DownloadService(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        public void QueueDownloads(IEnumerable<IDownloadViewModel> downloads)
        {
            foreach (IDownloadViewModel download in downloads)
            {
                _downloadQueue.Enqueue(download);
            }

            for (int downloadsStarted = _currentDownloads.Count; downloadsStarted < MaxConcurrentDownloads && _downloadQueue.Count > 0; ++downloadsStarted)
            {
                DownloadNext();
            }
        }

        public void TerminateAllDownloads()
        {
            _currentDownloads.ToArray().Apply(download => download.Kill());
        }

        private void DownloadNext()
        {
            if (_downloadQueue.Count == 0)
            {
                return;
            }

            IDownloadViewModel downloadViewModel = _downloadQueue.Dequeue();
            downloadViewModel.DownloadState = DownloadState.Downloading;

            Download download = new Download(downloadViewModel.VideoViewModel.Video, _settingsService.Settings);
            downloadViewModel.Download = download;

            ProgressMonitor progressMonitor = new ProgressMonitor(download.Process);

            void DownloadCompleted(object sender, EventArgs e)
            {
                DetachDownload();
            }

            void DownloadPaused(object sender, EventArgs e)
            {
                progressMonitor.Pause();
                downloadViewModel.DownloadState = DownloadState.Paused;
            }

            void DownloadResumed(object sender, EventArgs e)
            {
                downloadViewModel.DownloadState = DownloadState.Downloading;
                progressMonitor.Resume(download.Process);
            }

            void DownloadKilled(object sender, EventArgs e)
            {
                DetachDownload();
            }

            void DetachDownload()
            {
                download.Completed -= DownloadCompleted;
                download.Paused -= DownloadPaused;
                download.Resumed -= DownloadResumed;
                download.Killed -= DownloadKilled;

                _currentDownloads.Remove(download);

                downloadViewModel.DownloadState = DownloadState.Completed;
            }

            download.Completed += DownloadCompleted;
            download.Paused += DownloadPaused;
            download.Resumed += DownloadResumed;
            download.Killed += DownloadKilled;

            download.Start();

            _currentDownloads.Add(download);

            DownloadProgress downloadProgress = new DownloadProgress();
            downloadViewModel.DownloadProgress = downloadProgress;

            void ProgressUpdated(object sender, ProgressUpdatedEventArgs e)
            {
                string GetStageText(int stage)
                {
                    if (stage == 0)
                    {
                        return "Gathering Data";
                    }

                    if (_settingsService.Settings.DownloadType == DownloadType.Audio)
                    {
                        switch (stage)
                        {
                            case 1:
                                return "Downloading Audio";

                            case 2:
                                return "Finalising";

                            default:
                                return "Unknown";
                        }
                    }

                    switch (stage)
                    {
                        case 1:
                            return "Downloading Video";

                        case 2:
                            return "Downloading Audio";

                        case 3:
                            return "Finalising";

                        default:
                            return "Unknown";
                    }
                }

                downloadProgress.StatusText = GetStageText(e.Stage);
                downloadProgress.DownloadSpeed = e.DownloadSpeed;
                downloadProgress.ProgressPercentage = e.DownloadPercentage;
                downloadProgress.TotalDownloadSize = e.TotalDownloadSize;
            }

            void FinishedMonitoring(object sender, EventArgs e)
            {
                progressMonitor.FinishedMonitoring -= FinishedMonitoring;
                progressMonitor.ProgressUpdated -= ProgressUpdated;
            }

            progressMonitor.FinishedMonitoring += FinishedMonitoring;
            progressMonitor.ProgressUpdated += ProgressUpdated;

            progressMonitor.Run();
        }
    }
}