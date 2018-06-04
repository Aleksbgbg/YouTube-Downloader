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

    internal class ConversionsTabViewModel : ProcessTabViewModel, IConversionsTabViewModel
    {
        private readonly IConversionService _conversionService;

        public ConversionsTabViewModel(IEventAggregator eventAggregator, IActionButtonFactory actionButtonFactory, IConversionService conversionService, IProcessDispatcherService processDispatcherService)
                : base(eventAggregator, processDispatcherService, processTransferType => processTransferType == ProcessTransferType.Convert)
        {
            _conversionService = conversionService;

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
            _conversionService.QueueConversion(processViewModels.Select(processViewModel => processViewModel.Process).Cast<ConvertProcess>());
        }
    }
}