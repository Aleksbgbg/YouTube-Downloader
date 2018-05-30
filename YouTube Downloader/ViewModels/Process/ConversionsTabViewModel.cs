namespace YouTube.Downloader.ViewModels.Process
{
    using System.Linq;

    using Caliburn.Micro;

    using YouTube.Downloader.Core;
    using YouTube.Downloader.Core.Downloading;
    using YouTube.Downloader.Extensions;
    using YouTube.Downloader.Services.Interfaces;
    using YouTube.Downloader.ViewModels.Interfaces.Process;

    internal class ConversionsTabViewModel : ProcessTabViewModel, IConversionsTabViewModel
    {
        private readonly IEventAggregator _eventAggregator;

        private readonly IConversionService _conversionService;

        public ConversionsTabViewModel(IEventAggregator eventAggregator, IConversionService conversionService) : base(eventAggregator, processTransferType => processTransferType == ProcessTransferType.Convert)
        {
            _eventAggregator = eventAggregator;
            _conversionService = conversionService;
        }

        private protected override void OnProcessesAdded(IProcessViewModel[] processViewModels)
        {
            _conversionService.QueueConversion(processViewModels.Select(processViewModel => processViewModel.Process).Cast<ConvertProcess>());
        }

        private protected override void OnProcessExited(IProcessViewModel processViewModel)
        {
            _eventAggregator.BeginPublishOnUIThread(new ProcessTransferMessage(ProcessTransferType.Complete, processViewModel.ToEnumerable()));
        }
    }
}