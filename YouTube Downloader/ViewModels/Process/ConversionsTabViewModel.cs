namespace YouTube.Downloader.ViewModels.Process
{
    using System.Linq;

    using Caliburn.Micro;

    using YouTube.Downloader.Core;
    using YouTube.Downloader.Core.Downloading;
    using YouTube.Downloader.Services.Interfaces;
    using YouTube.Downloader.ViewModels.Interfaces.Process;

    internal class ConversionsTabViewModel : ProcessTabViewModel, IConversionsTabViewModel
    {
        private readonly IConversionService _conversionService;

        public ConversionsTabViewModel(IEventAggregator eventAggregator, IConversionService conversionService, IProcessDispatcherService processDispatcherService)
                : base(eventAggregator, processDispatcherService, processTransferType => processTransferType == ProcessTransferType.Convert)
        {
            _conversionService = conversionService;
        }

        private protected override void OnProcessesAdded(IProcessViewModel[] processViewModels)
        {
            _conversionService.QueueConversion(processViewModels.Select(processViewModel => processViewModel.Process).Cast<ConvertProcess>());
        }
    }
}