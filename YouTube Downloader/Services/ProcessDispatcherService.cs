namespace YouTube.Downloader.Services
{
    using System;
    using System.IO;

    using Caliburn.Micro;

    using YouTube.Downloader.Core;
    using YouTube.Downloader.Core.Downloading;
    using YouTube.Downloader.Extensions;
    using YouTube.Downloader.Factories.Interfaces;
    using YouTube.Downloader.Models;
    using YouTube.Downloader.Services.Interfaces;
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

        public void Dispatch(IProcessViewModel processViewModel)
        {
            ProcessTransferType nextTransfer;
            IProcessViewModel dispatch;

            switch (processViewModel)
            {
                case IDownloadProcessViewModel _:
                    {
                        FileInfo fileInfo = new FileInfo((string)processViewModel.Process.ProcessMonitor.ParameterMonitorings["Destination"].Value);

                        if (_settings.OutputFormat == OutputFormat.Auto ||
                            _settings.OutputFormat == OutputFormat.Mp4 && fileInfo.Extension == ".mp4" ||
                            _settings.OutputFormat == OutputFormat.Mp3 && fileInfo.Extension == ".mp3")
                        {
                            nextTransfer = ProcessTransferType.Complete;
                            dispatch = _processFactory.MakeCompleteProcessViewModel(processViewModel.VideoViewModel);
                        }
                        else
                        {
                            nextTransfer = ProcessTransferType.Convert;

                            ConvertProgress convertProgress = new ConvertProgress(fileInfo.Length);
                            ConvertProcess convertProcess = new ConvertProcess(fileInfo.FullName, fileInfo.Extension, convertProgress);

                            dispatch = _processFactory.MakeConvertProcessViewModel(processViewModel.VideoViewModel, convertProcess, convertProgress);
                        }
                    }
                    break;

                case IConvertProcessViewModel _:
                    nextTransfer = ProcessTransferType.Complete;
                    dispatch = _processFactory.MakeCompleteProcessViewModel(processViewModel.VideoViewModel);
                    break;

                default:
                    throw new InvalidOperationException("Cannot dispatch non-download or non-convert process.");
            }

            _eventAggregator.BeginPublishOnUIThread(new ProcessTransferMessage(nextTransfer, dispatch.ToEnumerable()));
        }
    }
}