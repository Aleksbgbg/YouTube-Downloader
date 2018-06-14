namespace YouTube.Downloader.ViewModels.Process.Tabs
{
    using Caliburn.Micro;

    using YouTube.Downloader.Core;
    using YouTube.Downloader.Factories.Interfaces;
    using YouTube.Downloader.ViewModels.Interfaces;
    using YouTube.Downloader.ViewModels.Interfaces.Process.Entities;
    using YouTube.Downloader.ViewModels.Interfaces.Process.Tabs;

    internal class CompletedTabViewModel : ProcessTabViewModel<ICompleteProcessViewModel>, ICompletedTabViewModel
    {
        public CompletedTabViewModel(IEventAggregator eventAggregator, IActionButtonFactory actionButtonFactory) : base(eventAggregator)
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

        private protected override bool CanAccept(ProcessTransferType processTransferType)
        {
            return processTransferType == ProcessTransferType.Complete;
        }
    }
}