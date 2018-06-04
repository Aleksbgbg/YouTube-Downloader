namespace YouTube.Downloader.ViewModels.Interfaces.Process
{
    using System.Collections.Generic;

    using Caliburn.Micro;

    internal interface IProcessTabViewModel : IViewModelBase
    {
        IObservableCollection<IProcessViewModel> Processes { get; }

        IObservableCollection<IProcessViewModel> SelectedProcesses { get; }

        void AddProcesses(IEnumerable<IProcessViewModel> processes);
    }
}