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

    internal class ConversionsTabViewModel : ActiveProcessTabViewModel, IConversionsTabViewModel
    {
        private readonly IConversionService _conversionService;

        public ConversionsTabViewModel(IEventAggregator eventAggregator, IActionButtonFactory actionButtonFactory, IConversionService conversionService, IProcessDispatcherService processDispatcherService)
                : base(eventAggregator, processDispatcherService)
        {
            _conversionService = conversionService;

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
            _conversionService.QueueConversion(processViewModels.Select(processViewModel => processViewModel.Process).Cast<ConvertProcess>());
        }

        private protected override bool CanAccept(ProcessTransferType processTransferType)
        {
            return processTransferType == ProcessTransferType.Convert;
        }
    }
}