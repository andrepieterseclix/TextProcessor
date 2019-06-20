
namespace TextProcessor.Infrastructure
{
    public enum InputStreamState
    {
        Stopped = 0,
        Stopping = 1,
        Starting = 2,
        Started = 3,
    }

    public enum LogEntryType
    {
        Info = 0,
        Warning = 1,
        Error = 2
    }
}
