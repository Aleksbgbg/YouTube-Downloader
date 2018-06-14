namespace YouTube.Downloader.ViewModels.Process.Tabs
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Data;

    using Caliburn.Micro;

    using YouTube.Downloader.Core;
    using YouTube.Downloader.ViewModels.Interfaces;
    using YouTube.Downloader.ViewModels.Interfaces.Process.Entities;
    using YouTube.Downloader.ViewModels.Interfaces.Process.Tabs;

    internal abstract class ProcessTabViewModel<T> : ViewModelBase, IProcessTabViewModel<T>, IHandle<ProcessTransferMessage>
            where T : class, IProcessViewModel
    {
        private protected ProcessTabViewModel(IEventAggregator eventAggregator)
        {
            DisplayName = GetType().Name.Replace("TabViewModel", string.Empty);

            eventAggregator.Subscribe(this);

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
            if (CanAccept(message.ProcessTransferType))
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

        private protected abstract bool CanAccept(ProcessTransferType processTransferType);
    }
}