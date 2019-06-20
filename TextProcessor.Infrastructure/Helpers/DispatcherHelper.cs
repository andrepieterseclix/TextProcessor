using System;
using System.Windows;
using System.Windows.Threading;

namespace TextProcessor.Infrastructure.Helpers
{
    public static class DispatcherHelper
    {
        static Dispatcher dispatcher = Application.Current.Dispatcher;

        public static void Execute(Action action)
        {
            if (dispatcher != null && !dispatcher.CheckAccess())
                dispatcher.Invoke(action);
            else
                action();
        }
    }
}
