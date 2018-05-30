namespace YouTube.Downloader.ViewModels.Process
{
    using System.IO;
    using System.Linq;

    using Caliburn.Micro;

    using YouTube.Downloader.Core;
    using YouTube.Downloader.Core.Downloading;
    using YouTube.Downloader.Extensions;
    using YouTube.Downloader.Models;
    using YouTube.Downloader.Services.Interfaces;
    using YouTube.Downloader.ViewModels.Interfaces.Process;

    internal class DownloadsTabViewModel : ProcessTabViewModel, IDownloadsTabViewModel
    {
        private readonly IEventAggregator _eventAggregator;

        private readonly IDownloadService _downloadService;

        private readonly Settings _settings;

        public DownloadsTabViewModel(IEventAggregator eventAggregator, IDownloadService downloadService, ISettingsService settingsService) : base(eventAggregator, processTransferType => processTransferType == ProcessTransferType.Download)
        {
            _eventAggregator = eventAggregator;
            _downloadService = downloadService;

            _settings = settingsService.Settings;
        }

        private protected override void OnProcessesAdded(IProcessViewModel[] processViewModels)
        {
            _downloadService.QueueDownloads(processViewModels.Select(processViewModel => processViewModel.Process).Cast<DownloadProcess>());
        }

        private protected override void OnProcessExited(IProcessViewModel processViewModel)
        {
            ProcessTransferType nextTransfer;

            string destination = (string)processViewModel.Process.ProcessMonitor.ParameterMonitorings["Destination"].Value;
            string extension = Path.GetExtension(destination);

            if (_settings.OutputFormat == OutputFormat.Auto ||
                _settings.OutputFormat == OutputFormat.Mp4 && extension == ".mp4" ||
                _settings.OutputFormat == OutputFormat.Mp3 && extension == ".mp3")
            {
                nextTransfer = ProcessTransferType.Complete;
            }
            else
            {
                processViewModel.Process = new ConvertProcess(destination, _settings.OutputFormat.ToString().ToLower(), processViewModel.Process.DownloadStatus);
                nextTransfer = ProcessTransferType.Convert;
            }

            _eventAggregator.BeginPublishOnUIThread(new ProcessTransferMessage(nextTransfer, processViewModel.ToEnumerable()));
        }
    }
}