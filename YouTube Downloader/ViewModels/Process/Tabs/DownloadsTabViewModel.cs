namespace YouTube.Downloader.ViewModels.Process.Tabs
{
    using System.Collections.Generic;
    using System.Linq;

    using Caliburn.Micro;

    using YouTube.Downloader.Factories.Interfaces;
    using YouTube.Downloader.Services.Interfaces;
    using YouTube.Downloader.Utilities;
    using YouTube.Downloader.Utilities.Downloading;
    using YouTube.Downloader.ViewModels.Interfaces;
    using YouTube.Downloader.ViewModels.Interfaces.Process.Entities;
    using YouTube.Downloader.ViewModels.Interfaces.Process.Tabs;

    internal class DownloadsTabViewModel : ActiveProcessTabViewModel, IDownloadsTabViewModel
    {
        private readonly IDownloadService _downloadService;

        public DownloadsTabViewModel(IEventAggregator eventAggregator, IActionButtonFactory actionButtonFactory, IDownloadService downloadService, IProcessDispatcherService processDispatcherService)
                : base(eventAggregator, processDispatcherService)
        {
            _downloadService = downloadService;

            Buttons = new IActionButtonViewModel[]
            {
                    actionButtonFactory.MakeActionButtonViewModel("Delete", "Kill", () =>
                    {
                        void KillProcesses(IEnumerable<IActiveProcessViewModel> processes)
                        {
                            foreach (MonitoredProcess conversionProcess in processes.ToArray().Select(process => process.Process))
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

        private protected override void OnProcessesAdded(IActiveProcessViewModel[] processViewModels)
        {
            base.OnProcessesAdded(processViewModels);
            _downloadService.QueueDownloads(processViewModels.Select(processViewModel => processViewModel.Process).Cast<DownloadProcess>());
        }

        private protected override bool CanAccept(ProcessTransferType processTransferType)
        {
            return processTransferType == ProcessTransferType.Download;
        }
    }
}