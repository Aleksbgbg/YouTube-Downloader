namespace YouTube.Downloader.ViewModels.Process
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Data;

    using Caliburn.Micro;

    using YouTube.Downloader.Core;
    using YouTube.Downloader.Services.Interfaces;
    using YouTube.Downloader.ViewModels.Interfaces;
    using YouTube.Downloader.ViewModels.Interfaces.Process;

    internal abstract class ProcessTabViewModel<T> : ViewModelBase, IProcessTabViewModel<T>, IHandle<ProcessTransferMessage>
            where T : class, IProcessViewModel
    {
        private readonly IProcessDispatcherService _processDispatcherService;

        private readonly Predicate<ProcessTransferType> _processTransferFilter;

        private protected ProcessTabViewModel(IEventAggregator eventAggregator, IProcessDispatcherService processDispatcherService, Predicate<ProcessTransferType> processTransferFilter)
        {
            DisplayName = GetType().Name.Replace("TabViewModel", string.Empty);

            eventAggregator.Subscribe(this);

            _processDispatcherService = processDispatcherService;
            _processTransferFilter = processTransferFilter;

            ((ListCollectionView)CollectionViewSource.GetDefaultView(Processes)).CustomSort = Comparer<T>.Create((first, second) => -first.DownloadState.CompareTo(second.DownloadState));
        }

        public sealed override string DisplayName
        {
            get => base.DisplayName;

            set => base.DisplayName = value;
        }

        public IActionButtonViewModel[] Buttons { get; private protected set; }

        public IObservableCollection<T> Processes { get; } = new BindableCollection<T>();

        public IObservableCollection<T> SelectedProcesses { get; } = new BindableCollection<T>();

        public void Handle(ProcessTransferMessage message)
        {
            if (_processTransferFilter(message.ProcessTransferType))
            {
                AddProcesses(message.Processes.Cast<T>());
            }
        }

        public void AddProcesses(IEnumerable<T> processes)
        {
            T[] processViewModels = processes.ToArray();

            Processes.AddRange(processViewModels);
            OnProcessesAdded(processViewModels);
        }

        private protected virtual void OnProcessesAdded(T[] processViewModels)
        {
        }

        private protected virtual void OnProcessExited(T processViewModel)
        {
            _processDispatcherService.Dispatch(processViewModel);
        }
    }
}