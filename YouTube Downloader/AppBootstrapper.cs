namespace YouTube.Downloader
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;

    using Caliburn.Micro;

    using YouTube.Downloader.Core.Downloading;
    using YouTube.Downloader.Factories;
    using YouTube.Downloader.Factories.Interfaces;
    using YouTube.Downloader.Models;
    using YouTube.Downloader.Services;
    using YouTube.Downloader.Services.Interfaces;
    using YouTube.Downloader.Utilities;
    using YouTube.Downloader.Utilities.Interfaces;
    using YouTube.Downloader.ViewModels;
    using YouTube.Downloader.ViewModels.Interfaces;

    internal class AppBootstrapper : BootstrapperBase
    {
        private readonly SimpleContainer _container = new SimpleContainer();

        internal AppBootstrapper()
        {
            Initialize();
        }

        protected override void BuildUp(object instance)
        {
            _container.BuildUp(instance);
        }

        protected override void Configure()
        {
            // Register Services
            _container.Singleton<IWindowManager, WindowManager>();
            _container.Singleton<IEventAggregator, EventAggregator>();

            _container.Singleton<IAppDataService, AppDataService>();
            _container.Singleton<IDataService, DataService>();
            _container.Singleton<IDialogService, DialogService>();
            _container.Singleton<IDownloadService, DownloadService>();
            _container.Singleton<ISettingsService, SettingsService>();
            _container.Singleton<IYouTubeApiService, YouTubeApiService>();

            _container.Singleton<IFileSystemUtility, FileSystemUtility>();

            // Register Factories
            _container.Singleton<IDownloadFactory, DownloadFactory>();
            _container.Singleton<IRequeryFactory, RequeryFactory>();
            _container.Singleton<IVideoFactory, VideoFactory>();

            // Register ViewModels
            _container.Singleton<IShellViewModel, ShellViewModel>();
            _container.Singleton<IMainViewModel, MainViewModel>();

            _container.Singleton<IQueryViewModel, QueryViewModel>();
            _container.Singleton<IRequeryViewModel, RequeryViewModel>();
            _container.Singleton<IMatchedVideosViewModel, MatchedVideosViewModel>();
            _container.PerRequest<IMatchedVideoViewModel, MatchedVideoViewModel>();

            _container.Singleton<ICurrentDownloadsViewModel, CurrentDownloadsViewModel>();
            _container.PerRequest<IDownloadViewModel, DownloadViewModel>();

            _container.PerRequest<IVideoViewModel, VideoViewModel>();

            _container.Singleton<ISettingsViewModel, SettingsViewModel>();
        }

        protected override object GetInstance(Type service, string key)
        {
            return _container.GetInstance(service, key) ?? throw new InvalidOperationException("Service is not registered.");
        }

        protected override IEnumerable<object> GetAllInstances(Type serviceType)
        {
            return _container.GetAllInstances(serviceType);
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            IDataService dataService = IoC.Get<IDataService>();

            IoC.Get<IQueryViewModel>().Query = dataService.LoadAndWipe<string>("Query") ?? string.Empty;
            IoC.Get<IMatchedVideosViewModel>().Load(dataService.LoadAndWipe<IEnumerable<YouTubeVideo>>("Matched Videos", "[]"));
            IoC.Get<ICurrentDownloadsViewModel>().AddDownloads(dataService.LoadAndWipe<IEnumerable<YouTubeVideo>>("Downloads", "[]").Select(IoC.Get<IVideoFactory>().MakeVideoViewModel));

            DisplayRootViewFor<IShellViewModel>();
        }

        protected override void OnExit(object sender, System.EventArgs e)
        {
            IDataService dataService = IoC.Get<IDataService>();

            dataService.Save("Query", IoC.Get<IQueryViewModel>().Query);
            dataService.Save("Matched Videos", IoC.Get<IMatchedVideosViewModel>().Videos.Select(matchedVideoViewModel => matchedVideoViewModel.VideoViewModel.Video));

            Download[] downloads = IoC.Get<IDownloadService>().Downloads.ToArray();
            dataService.Save("Downloads", downloads.Select(download => download.YouTubeVideo));
            downloads.Apply(download => download.Kill());
        }
    }
}