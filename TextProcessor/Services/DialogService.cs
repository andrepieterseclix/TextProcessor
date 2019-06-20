using System;
using System.ComponentModel.Composition;
using System.Windows;
using TextProcessor.Interfaces;
using TextProcessor.Views;

namespace TextProcessor.Services
{
    [Export(typeof(IDialogService))]
    class DialogService : IDialogService
    {
        HelpWindow helpWindow;

        public void ShowInfo(string caption, string messageFormat, params object[] args)
        {
            MessageBox.Show(string.Format(messageFormat, args), caption, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public bool AskQuestion(string caption, string messageFormat, params object[] args)
        {
            return MessageBox.Show(string.Format(messageFormat, args), caption, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
        }

        public void ShowHelpWindow(bool modal)
        {
            if (helpWindow != null)
            {
                if (helpWindow.WindowState == WindowState.Minimized)
                    helpWindow.WindowState = WindowState.Normal;
                helpWindow.Focus();
                return;
            }

            helpWindow = new HelpWindow();
            helpWindow.Closed += helpWindow_Closed;
            if (modal)
                helpWindow.ShowDialog();
            else
                helpWindow.Show();
        }

        void helpWindow_Closed(object sender, EventArgs e)
        {
            helpWindow.Closed -= helpWindow_Closed;
            helpWindow = null;
        }
    }
}
