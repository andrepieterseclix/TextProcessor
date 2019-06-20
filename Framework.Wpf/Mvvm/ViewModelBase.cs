using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Framework.Wpf.Mvvm
{
    public class ViewModelBase : BindableBase
    {
        static Dispatcher dispatcher = Application.Current.Dispatcher;

        protected void Invoke(Action action)
        {
            if (dispatcher.CheckAccess())
                action();
            else
                dispatcher.Invoke(action);
        }

        protected ICommand CreateCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            return new DelegateCommand(execute, canExecute);
        }
    }
}
