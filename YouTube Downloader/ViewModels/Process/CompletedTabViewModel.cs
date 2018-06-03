namespace YouTube.Downloader.ViewModels.Process
{
    using Caliburn.Micro;

    using YouTube.Downloader.Core;
    using YouTube.Downloader.Factories.Interfaces;
    using YouTube.Downloader.ViewModels.Interfaces;
    using YouTube.Downloader.ViewModels.Interfaces.Process;

    internal class CompletedTabViewModel : ProcessTabViewModel, ICompletedTabViewModel
    {
        public CompletedTabViewModel(IEventAggregator eventAggregator, IActionButtonFactory actionButtonFactory)
                : base(eventAggregator, null, processTransferType => processTransferType == ProcessTransferType.Complete)
        {
            Buttons = new IActionButtonViewModel[]
            {
                    actionButtonFactory.MakeActionButtonViewModel("Delete", "Remove", () =>
                    {
                        if (SelectedProcesses.Count == 0)
                        {
                            Processes.Clear();
                            return;
                        }

                        Processes.RemoveRange(SelectedProcesses);
                    })
            };
        }
    }
}