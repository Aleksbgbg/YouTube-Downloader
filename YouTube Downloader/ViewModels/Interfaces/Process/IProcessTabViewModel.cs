namespace YouTube.Downloader.ViewModels.Interfaces.Process
{
    using System.Collections.Generic;

    using Caliburn.Micro;

    internal interface IProcessTabViewModel<T> : IViewModelBase
            where T : class, IProcessViewModel
    {
        IObservableCollection<T> Processes { get; }

        IObservableCollection<T> SelectedProcesses { get; }

        void AddProcesses(IEnumerable<T> processes);
    }
}