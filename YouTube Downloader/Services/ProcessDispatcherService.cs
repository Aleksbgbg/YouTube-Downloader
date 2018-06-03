namespace YouTube.Downloader.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Caliburn.Micro;

    using YouTube.Downloader.Core;
    using YouTube.Downloader.Core.Downloading;
    using YouTube.Downloader.Extensions;
    using YouTube.Downloader.Factories.Interfaces;
    using YouTube.Downloader.Models;
    using YouTube.Downloader.Models.Download;
    using YouTube.Downloader.Services.Interfaces;
    using YouTube.Downloader.ViewModels.Interfaces;
    using YouTube.Downloader.ViewModels.Interfaces.Process;

    internal class ProcessDispatcherService : IProcessDispatcherService
    {
        private readonly IEventAggregator _eventAggregator;

        private readonly IProcessFactory _processFactory;

        private readonly Settings _settings;

        public ProcessDispatcherService(IEventAggregator eventAggregator, IProcessFactory processFactory, ISettingsService settingsService)
        {
            _eventAggregator = eventAggregator;
            _processFactory = processFactory;
            _settings = settingsService.Settings;
        }

        public void Dispatch(IEnumerable<IViewModelBase> viewModels)
        {
            foreach (IViewModelBase viewModel in viewModels)
            {
                Dispatch(viewModel);
            }
        }

        public void Dispatch(IViewModelBase viewModel)
        {
            ProcessTransferType nextTransfer;
            IProcessViewModel dispatch;

            switch (viewModel)
            {
                case IVideoViewModel videoViewModel:
                    {
                        string videoTitle = videoViewModel.Video.Title;

                        if (Directory.GetFiles(_settings.DownloadPath).Select(Path.GetFileNameWithoutExtension).Any(filename => filename == videoTitle))
                        {
                            nextTransfer = ProcessTransferType.Complete;
                            dispatch = _processFactory.MakeCompleteProcessViewModel(videoViewModel);
                        }
                        else
                        {
                            nextTransfer = ProcessTransferType.Download;
                            dispatch = _processFactory.MakeDownloadProcessViewModel(videoViewModel);
                        }
                    }
                    break;

                case IDownloadProcessViewModel downloadProcessViewModel:
                    {
                        FileInfo fileInfo = new FileInfo((string)downloadProcessViewModel.Process.ProcessMonitor.ParameterMonitorings["Destination"].Value);

                        if (_settings.OutputFormat == OutputFormat.Auto ||
                            _settings.OutputFormat == OutputFormat.Mp4 && fileInfo.Extension == ".mp4" ||
                            _settings.OutputFormat == OutputFormat.Mp3 && fileInfo.Extension == ".mp3")
                        {
                            nextTransfer = ProcessTransferType.Complete;
                            dispatch = _processFactory.MakeCompleteProcessViewModel(downloadProcessViewModel.VideoViewModel);
                        }
                        else
                        {
                            nextTransfer = ProcessTransferType.Convert;

                            ConvertProgress convertProgress = new ConvertProgress(fileInfo.Length);
                            ConvertProcess convertProcess = new ConvertProcess(fileInfo.FullName, _settings.OutputFormat.ToString().ToLower(), convertProgress);

                            dispatch = _processFactory.MakeConvertProcessViewModel(downloadProcessViewModel.VideoViewModel, convertProcess, convertProgress);
                        }
                    }
                    break;

                case IConvertProcessViewModel convertProcessViewModel:
                    nextTransfer = ProcessTransferType.Complete;
                    dispatch = _processFactory.MakeCompleteProcessViewModel(convertProcessViewModel.VideoViewModel);
                    break;

                default:
                    throw new InvalidOperationException("Cannot dispatch non-download or non-convert process.");
            }

            void ProcessStarted(object sender, EventArgs e)
            {
                switch (dispatch)
                {
                    case IDownloadProcessViewModel _:
                        dispatch.DownloadState = DownloadState.Downloading;
                        break;

                    case IConvertProcessViewModel _:
                        dispatch.DownloadState = DownloadState.Converting;
                        break;
                }
            }

            void ProcessExited(object sender, EventArgs e)
            {
                dispatch.Process.Started -= ProcessStarted;
                dispatch.Process.Exited -= ProcessExited;
            }

            if (dispatch.Process != null)
            {
                dispatch.Process.Started += ProcessStarted;
                dispatch.Process.Exited += ProcessExited;
            }

            _eventAggregator.BeginPublishOnUIThread(new ProcessTransferMessage(nextTransfer, dispatch.ToEnumerable()));
        }
    }
}