using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace TextProcessor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var loader = new ApplicationLoader();
            loader.Initialize();
            base.MainWindow = loader.ApplicationWindow;
            base.MainWindow.Show();
            base.ShutdownMode = ShutdownMode.OnMainWindowClose;
        }
    }
}
