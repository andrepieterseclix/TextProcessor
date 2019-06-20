using System;

namespace TextProcessor.Infrastructure.Services
{
    public interface ILogService
    {
        void Write(string format, params object[] args);

        void Write(LogEntryType entryType, string format, params object[] args);
    }
}
