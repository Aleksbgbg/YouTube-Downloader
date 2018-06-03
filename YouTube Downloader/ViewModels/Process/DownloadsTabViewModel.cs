namespace YouTube.Downloader.ViewModels.Process
{
    using System.Collections.Generic;
    using System.Linq;

    using Caliburn.Micro;

    using YouTube.Downloader.Core;
    using YouTube.Downloader.Core.Downloading;
    using YouTube.Downloader.Factories.Interfaces;
    using YouTube.Downloader.Services.Interfaces;
    using YouTube.Downloader.ViewModels.Interfaces;
    using YouTube.Downloader.ViewModels.Interfaces.Process;

    internal class DownloadsTabViewModel : ProcessTabViewModel, IDownloadsTabViewModel
    {
        private readonly IDownloadService _downloadService;

        public DownloadsTabViewModel(IEventAggregator eventAggregator, IActionButtonFactory actionButtonFactory, IDownloadService downloadService, IProcessDispatcherService processDispatcherService)
                : base(eventAggregator, processDispatcherService, processTransferType => processTransferType == ProcessTransferType.Download)
        {
            _downloadService = downloadService;

            Buttons = new IActionButtonViewModel[]
            {
                    actionButtonFactory.MakeActionButtonViewModel("Delete", "Kill", () =>
                    {
                        void KillProcesses(IEnumerable<IProcessViewModel> processes)
                        {
                            foreach (MonitoredProcess conversionProcess in processes.Select(process => process.Process))
                            {
                                conversionProcess.Kill();
                            }
                        }

                        if (SelectedProcesses.Count == 0)
                        {
                            KillProcesses(Processes);
                            return;
                        }

                        KillProcesses(SelectedProcesses);
                    })
            };
        }

        private protected override void OnProcessesAdded(IProcessViewModel[] processViewModels)
        {
            _downloadService.QueueDownloads(processViewModels.Select(processViewModel => processViewModel.Process).Cast<DownloadProcess>());
        }
    }
}