using Framework.Wpf.Mvvm;
using System;
using TextProcessor.Infrastructure.Services;

namespace TextProcessor.ViewModels
{
    class TextItemViewModel : ViewModelBase
    {
        string text;

        bool isHighlighted;

        bool isSelected;

        DateTime timeStamp;

        IOutputService outputService;

        public TextItemViewModel(string text, IOutputService outputHelper)
        {
            Text = text;
            IsSelected = true;
            TimeStamp = DateTime.Now;

            // set outputService last so that it does not call refresh when setting IsSelected
            outputService = outputHelper;
        }

        public string Text
        {
            get { return text; }
            set { SetProperty(ref text, value); }
        }

        public bool IsHighlighted
        {
            get { return isHighlighted; }
            set { SetProperty(ref isHighlighted, value); }
        }

        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                if (SetProperty(ref isSelected, value) && outputService != null)
                    outputService.Refresh();
            }
        }

        public DateTime TimeStamp
        {
            get { return timeStamp; }
            private set { SetProperty(ref timeStamp, value); }
        }
    }
}
