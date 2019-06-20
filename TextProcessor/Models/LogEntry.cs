using TextProcessor.Infrastructure;

namespace TextProcessor.Models
{
    class LogEntry
    {
        internal LogEntry(LogEntryType entryType, string text)
        {
            EntryType = entryType;
            Text = text;
        }

        public LogEntryType EntryType { get; private set; }

        public string Text { get; private set; }
    }
}
