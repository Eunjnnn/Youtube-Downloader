﻿namespace WPF
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;

    using Caliburn.Micro;

    using WPF.Factories;
    using WPF.Factories.Interfaces;
    using WPF.Models;
    using WPF.Services;
    using WPF.Services.Interfaces;
    using WPF.ViewModels;
    using WPF.ViewModels.Interfaces;
    using WPF.ViewModels.Interfaces.Process;
    using WPF.ViewModels.Interfaces.Process.Entities;
    using WPF.ViewModels.Interfaces.Process.Tabs;
    using WPF.ViewModels.Process;
    using WPF.ViewModels.Process.Entities;
    using WPF.ViewModels.Process.Tabs;


    internal class AppBootstrapper : BootstrapperBase
    {
        private readonly SimpleContainer _container = new SimpleContainer();

        internal AppBootstrapper()
        {
            Initialize();

            ViewLocator.NameTransformer.AddRule(@"^YouTube.Downloader.ViewModels.Process(?:\.\w*)+(\.\w*ProcessViewModel)", "YouTube.Downloader.Views.Process.ProcessView");
            ViewLocator.NameTransformer.AddRule(@"^YouTube.Downloader.ViewModels.Process(?:\.\w*)+(\.\w*TabViewModel)", "YouTube.Downloader.Views.Process.ProcessTabView");
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
            _container.Singleton<IDialogService, DialogService>();
            _container.Singleton<IDownloadService, DownloadService>();
            _container.Singleton<IFileSystemService, FileSystemService>();
            _container.Singleton<IProcessDispatcherService, ProcessDispatcherService>();
            _container.Singleton<ISettingsService, SettingsService>();
            _container.Singleton<IYouTubeApiService, YouTubeApiService>();

            // Register Factories
            _container.Singleton<IActionButtonFactory, ActionButtonFactory>();
            _container.Singleton<IProcessFactory, ProcessFactory>();
            _container.Singleton<IRequeryFactory, RequeryFactory>();
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
            IDataService dataService = _container.GetInstance<IDataService>();

            _container.GetInstance<IQueryViewModel>().Query = dataService.LoadAndWipe<string>("Query") ?? string.Empty;
            _container.GetInstance<IMatchedVideosViewModel>().Load(dataService.LoadAndWipe<IEnumerable<QueryResult>>("Matched Videos", "[]"));

            DisplayRootViewFor<IShellViewModel>();
        }

        protected override void OnExit(object sender, System.EventArgs e)
        {
            IDataService dataService = _container.GetInstance<IDataService>();

            dataService.Save("Query", _container.GetInstance<IQueryViewModel>().Query);
            dataService.Save("Matched Videos", _container.GetInstance<IMatchedVideosViewModel>().Videos.Select(matchedVideoViewModel => matchedVideoViewModel.QueryResult));
        }
    }
}