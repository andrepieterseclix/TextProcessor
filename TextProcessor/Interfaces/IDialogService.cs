
namespace TextProcessor.Interfaces
{
    interface IDialogService
    {
        void ShowInfo(string caption, string messageFormat, params object[] args);

        bool AskQuestion(string caption, string messageFormat, params object[] args);

        void ShowHelpWindow(bool modal);
    }
}
