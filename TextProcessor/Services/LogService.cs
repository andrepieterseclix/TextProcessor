using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using TextProcessor.Infrastructure;
using TextProcessor.Infrastructure.Helpers;
using TextProcessor.Infrastructure.Services;
using TextProcessor.Interfaces;
using TextProcessor.Models;

namespace TextProcessor.Services
{
    [Export(typeof(ILogService))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    class LogService : ILogService, IObservableLogger
    {
        internal LogService()
        {
            Entries = new ObservableCollection<LogEntry>();
        }

        public ObservableCollection<LogEntry> Entries { get; private set; }

        public void Write(string format, params object[] args)
        {
            Write(LogEntryType.Info, format, args);
        }

        public void Write(LogEntryType entryType, string format, params object[] args)
        {
            var text = string.Format(format, args);

            // handle thread safety here, otherwise it would have been handled everywhere where calling the write method
            DispatcherHelper.Execute(() => Entries.Add(new LogEntry(entryType, text)));
        }
    }
}
