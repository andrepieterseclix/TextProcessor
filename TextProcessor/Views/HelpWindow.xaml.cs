using System;
using System.IO;
using System.Windows;

namespace TextProcessor.Views
{
    /// <summary>
    /// Interaction logic for HelpWindow.xaml
    /// </summary>
    public partial class HelpWindow : Window
    {
        public HelpWindow()
        {
            InitializeComponent();
            navigateToHelp();
        }

        void navigateToHelp()
        {
            try
            {
                var source = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Help\index.html");
                if (!File.Exists(source))
                    throw new FileNotFoundException("The help content could not be loaded because the file does not exist!");
                browser.Navigate(source);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("The help system could not be loaded.  {0}", ex.Message), "Help Error", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }
        }
    }
}
