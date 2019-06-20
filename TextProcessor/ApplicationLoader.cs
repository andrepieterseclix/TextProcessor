using System;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Windows;
using TextProcessor.Infrastructure.Services;
using TextProcessor.Interfaces;
using TextProcessor.Services;
using TextProcessor.ViewModels;
using TextProcessor.Views;

namespace TextProcessor
{
    public class ApplicationLoader
    {
        readonly string pluginsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins");

        PluginsService pluginsService;

        ILogService logService;

        IStreamService streamService;

        IOutputService outputService;

        IDialogService dialogService;

        internal void Initialize()
        {
            ensurePathsExist();
            createContainer();
            loadDependencies();
            createApplicationWindow();
        }

        public CompositionContainer Container { get; private set; }

        public Window ApplicationWindow { get; private set; }

        void ensurePathsExist()
        {
            if (!Directory.Exists(pluginsDirectory))
                Directory.CreateDirectory(pluginsDirectory);
        }

        void createContainer()
        {
            // create and configure catalogs
            AggregateCatalog catalog = new AggregateCatalog();
            DirectoryCatalog directoryCatalog = new DirectoryCatalog(pluginsDirectory);
            catalog.Catalogs.Add(directoryCatalog);
            AssemblyCatalog assemblyCatalog = new AssemblyCatalog(typeof(ApplicationLoader).Assembly);
            catalog.Catalogs.Add(assemblyCatalog);

            // instantiate the container
            Container = new CompositionContainer(catalog);
        }

        void loadDependencies()
        {
            // resolve services
            logService = Container.GetExportedValue<ILogService>();
            streamService = Container.GetExportedValue<IStreamService>();
            outputService = Container.GetExportedValue<IOutputService>();
            dialogService = Container.GetExportedValue<IDialogService>();

            // resolve the plugins service instance from the current assembly, which will in turn resolve the plugins automatically from the plugins folder
            pluginsService = Container.GetExportedValue<PluginsService>();
            pluginsService.InitializeStreams();
            pluginsService.InitializeProcessors();
        }

        void createApplicationWindow()
        {
            var viewModel = new MainViewModel(pluginsService.InputStreams, pluginsService.TextProcessors, logService, streamService, outputService, dialogService);
            this.ApplicationWindow = new MainWindow() { DataContext = viewModel };
            logService.Write("Application window loaded successfully...");
        }
    }
}
