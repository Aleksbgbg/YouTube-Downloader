namespace YouTube.Downloader.ViewModels.Process
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows.Data;

    using Caliburn.Micro;

    using YouTube.Downloader.Core;
    using YouTube.Downloader.Models.Download;
    using YouTube.Downloader.Services.Interfaces;
    using YouTube.Downloader.ViewModels.Interfaces.Process;

    internal abstract class ProcessTabViewModel : ViewModelBase, IProcessTabViewModel, IHandle<ProcessTransferMessage>
    {
        private readonly IProcessDispatcherService _processDispatcherService;

        private readonly Predicate<ProcessTransferType> _processTransferFilter;

        private protected ProcessTabViewModel(IEventAggregator eventAggregator, IProcessDispatcherService processDispatcherService, Predicate<ProcessTransferType> processTransferFilter)
        {
            DisplayName = GetType().Name.Replace("TabViewModel", string.Empty);

            eventAggregator.Subscribe(this);

            _processDispatcherService = processDispatcherService;
            _processTransferFilter = processTransferFilter;

            ((ListCollectionView)CollectionViewSource.GetDefaultView(Processes)).CustomSort = Comparer<IProcessViewModel>.Create((first, second) => -first.DownloadStatus.DownloadState.CompareTo(second.DownloadStatus.DownloadState));
        }

        public sealed override string DisplayName
        {
            get => base.DisplayName;

            set => base.DisplayName = value;
        }

        public IObservableCollection<IProcessViewModel> Processes { get; } = new BindableCollection<IProcessViewModel>();

        public IObservableCollection<IProcessViewModel> SelectedProcesses { get; } = new BindableCollection<IProcessViewModel>();

        public void Handle(ProcessTransferMessage message)
        {
            if (_processTransferFilter(message.ProcessTransferType))
            {
                AddProcesses(message.Processes);
            }
        }

        public void AddProcesses(IEnumerable<IProcessViewModel> processes)
        {
            IProcessViewModel[] processViewModels = processes.ToArray();

            foreach (IProcessViewModel processViewModel in processViewModels)
            {
                void ProcessExited(object sender, EventArgs e)
                {
                    processViewModel.Process.Exited -= ProcessExited;
                    processViewModel.DownloadStatus.PropertyChanged -= DownloadStatusPropertyChanged;

                    SelectedProcesses.Remove(processViewModel);
                    Processes.Remove(processViewModel);

                    OnProcessExited(processViewModel);
                }

                void DownloadStatusPropertyChanged(object sender, PropertyChangedEventArgs e)
                {
                    if (e.PropertyName == nameof(DownloadStatus.DownloadState))
                    {
                        Processes.Refresh();
                    }
                }

                processViewModel.Process.Exited += ProcessExited;
                processViewModel.DownloadStatus.PropertyChanged += DownloadStatusPropertyChanged;
            }

            Processes.AddRange(processViewModels);
            OnProcessesAdded(processViewModels);
        }

        private protected virtual void OnProcessesAdded(IProcessViewModel[] processViewModels)
        {
        }

        private protected virtual void OnProcessExited(IProcessViewModel processViewModel)
        {
            _processDispatcherService.Dispatch(processViewModel);
        }
    }
}