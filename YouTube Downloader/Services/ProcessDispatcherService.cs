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
    using YouTube.Downloader.ViewModels.Interfaces.Process.Entities;

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

            void DispatchToDownload(IVideoViewModel videoViewModel)
            {
                nextTransfer = ProcessTransferType.Download;
                dispatch = _processFactory.MakeDownloadProcessViewModel(videoViewModel);
            }

            void DispatchToComplete(IVideoViewModel videoViewModel)
            {
                nextTransfer = ProcessTransferType.Complete;
                dispatch = _processFactory.MakeCompleteProcessViewModel(videoViewModel);
            }

            switch (viewModel)
            {
                case IVideoViewModel videoViewModel:
                    {
                        string videoTitle = videoViewModel.Video.Title;

                        if (Directory.GetFiles(_settings.DownloadPath).Select(Path.GetFileNameWithoutExtension).Any(filename => filename == videoTitle))
                        {
                            DispatchToComplete(videoViewModel);
                        }
                        else
                        {
                            DispatchToDownload(videoViewModel);
                        }
                    }
                    break;

                case IDownloadProcessViewModel downloadProcessViewModel:
                    {
                        if (!((DownloadProcess)downloadProcessViewModel.Process).DidComplete)
                        {
                            DispatchToComplete(downloadProcessViewModel.VideoViewModel);
                            break;
                        }

                        string destinationFilename = (string)downloadProcessViewModel.Process.ProcessMonitor.ParameterMonitorings["Destination"].Value;

                        FileInfo fileInfo = new FileInfo(destinationFilename);

                        if (_settings.OutputFormat == OutputFormat.Auto ||
                            _settings.OutputFormat == OutputFormat.Mp4 && fileInfo.Extension == ".mp4" ||
                            _settings.OutputFormat == OutputFormat.Mp3 && fileInfo.Extension == ".mp3")
                        {
                            DispatchToComplete(downloadProcessViewModel.VideoViewModel);
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
                    DispatchToComplete(convertProcessViewModel.VideoViewModel);
                    break;

                default:
                    throw new InvalidOperationException("Cannot dispatch non-download or non-convert process.");
            }

            if (dispatch is IActiveProcessViewModel activeProcessViewModel)
            {
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
                    activeProcessViewModel.Process.Started -= ProcessStarted;
                    activeProcessViewModel.Process.Exited -= ProcessExited;
                }

                activeProcessViewModel.Process.Started += ProcessStarted;
                activeProcessViewModel.Process.Exited += ProcessExited;
            }

            _eventAggregator.BeginPublishOnUIThread(new ProcessTransferMessage(nextTransfer, dispatch.ToEnumerable()));
        }
    }
}