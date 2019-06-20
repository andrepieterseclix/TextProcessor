using System.Collections.ObjectModel;
using TextProcessor.Models;

namespace TextProcessor.Interfaces
{
    interface IObservableLogger
    {
        ObservableCollection<LogEntry> Entries { get; }
    }
}
