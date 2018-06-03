namespace YouTube.Downloader
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;

    using Caliburn.Micro;

    using YouTube.Downloader.Factories;
    using YouTube.Downloader.Factories.Interfaces;
    using YouTube.Downloader.Models;
    using YouTube.Downloader.Services;
    using YouTube.Downloader.Services.Interfaces;
    using YouTube.Downloader.Utilities;
    using YouTube.Downloader.Utilities.Interfaces;
    using YouTube.Downloader.ViewModels;
    using YouTube.Downloader.ViewModels.Interfaces;
    using YouTube.Downloader.ViewModels.Interfaces.Process;
    using YouTube.Downloader.ViewModels.Process;

    internal class AppBootstrapper : BootstrapperBase
    {
        private readonly SimpleContainer _container = new SimpleContainer();

        internal AppBootstrapper()
        {
            Initialize();

            Func<Type, DependencyObject, object, Type> originalLocator = ViewLocator.LocateTypeForModelType;

            ViewLocator.LocateTypeForModelType = (modelType, displayLocation, context) =>
            {
                if (modelType.Name.EndsWith("ProcessViewModel"))
                {
                    return AssemblySource.FindTypeByNames(new string[]
                    {
                            $"{modelType.Namespace.Replace("Model", string.Empty)}.ProcessView"
                    });
                }

                return originalLocator(modelType, displayLocation, context);
            };
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
            _container.Singleton<IConversionService, ConversionService>();
            _container.Singleton<IDataService, DataService>();
            _container.Singleton<IDownloadService, DownloadService>();
            _container.Singleton<IProcessDispatcherService, ProcessDispatcherService>();
            _container.Singleton<ISettingsService, SettingsService>();
            _container.Singleton<IYouTubeApiService, YouTubeApiService>();

            _container.Singleton<IFileSystemUtility, FileSystemUtility>();

            // Register Factories
            _container.Singleton<IActionButtonFactory, ActionButtonFactory>();
            _container.Singleton<IProcessFactory, ProcessFactory>();
            _container.Singleton<IVideoFactory, VideoFactory>();

            // Register ViewModels
            _container.Singleton<IShellViewModel, ShellViewModel>();
            _container.Singleton<IMainViewModel, MainViewModel>();

            _container.PerRequest<IActionButtonViewModel, ActionButtonViewModel>();

            _container.Singleton<IQueryViewModel, QueryViewModel>();
            _container.Singleton<IRequeryViewModel, RequeryViewModel>();
            _container.Singleton<IMatchedVideosViewModel, MatchedVideosViewModel>();
            _container.PerRequest<IMatchedVideoViewModel, MatchedVideoViewModel>();

            _container.Singleton<IProcessTabsViewModel, ProcessTabsViewModel>();
            _container.Singleton<IDownloadsTabViewModel, DownloadsTabViewModel>();
            _container.Singleton<IConversionsTabViewModel, ConversionsTabViewModel>();
            _container.Singleton<ICompletedTabViewModel, CompletedTabViewModel>();

            _container.PerRequest<IDownloadProcessViewModel, DownloadProcessViewModel>();
            _container.PerRequest<IConvertProcessViewModel, ConvertProcessViewModel>();
            _container.PerRequest<ICompleteProcessViewModel, CompleteProcessViewModel>();

            _container.PerRequest<IProcessViewModel, ProcessViewModel>();

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

            DisplayRootViewFor<IShellViewModel>();
        }

        protected override void OnExit(object sender, System.EventArgs e)
        {
            IDataService dataService = IoC.Get<IDataService>();

            dataService.Save("Query", IoC.Get<IQueryViewModel>().Query);
            dataService.Save("Matched Videos", IoC.Get<IMatchedVideosViewModel>().Videos.Select(matchedVideoViewModel => matchedVideoViewModel.VideoViewModel.Video));
        }
    }
}