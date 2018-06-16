namespace YouTube.Downloader.ViewModels.Process.Tabs
{
    using System;
    using System.ComponentModel;

    using Caliburn.Micro;

    using YouTube.Downloader.Models.Download;
    using YouTube.Downloader.Services.Interfaces;
    using YouTube.Downloader.ViewModels.Interfaces.Process.Entities;
    using YouTube.Downloader.ViewModels.Interfaces.Process.Tabs;

    internal abstract class ActiveProcessTabViewModel : ProcessTabViewModel<IActiveProcessViewModel>, IActiveProcessTabViewModel
    {
        private readonly IProcessDispatcherService _processDispatcherService;

        private protected ActiveProcessTabViewModel(IEventAggregator eventAggregator, IProcessDispatcherService processDispatcherService) : base(eventAggregator)
        {
            _processDispatcherService = processDispatcherService;
        }

        private protected override void OnProcessesAdded(IActiveProcessViewModel[] processViewModels)
        {
            foreach (IActiveProcessViewModel activeProcessViewModel in processViewModels)
            {
                void ProcessExited(object sender, EventArgs e)
                {
                    activeProcessViewModel.Process.Exited -= ProcessExited;
                    activeProcessViewModel.PropertyChanged -= DownloadStatusPropertyChanged;

                    OnUIThread(() =>
                    {
                        SelectedProcesses.Remove(activeProcessViewModel);
                        Processes.Remove(activeProcessViewModel);
                    });

                    OnProcessExited(activeProcessViewModel);
                }

                void DownloadStatusPropertyChanged(object sender, PropertyChangedEventArgs e)
                {
                    if (e.PropertyName == nameof(DownloadStatus.DownloadState))
                    {
                        Processes.Refresh();
                    }
                }

                activeProcessViewModel.Process.Exited += ProcessExited;
                activeProcessViewModel.PropertyChanged += DownloadStatusPropertyChanged;
            }
        }

        private protected virtual void OnProcessExited(IActiveProcessViewModel processViewModel)
        {
            _processDispatcherService.Dispatch(processViewModel);
        }
    }
}