using System.Linq;

namespace TextProcessor.Processors.ViewModels
{
    class RegexProcessorViewModel : BaseProcessorViewModel
    {
        bool filter;

        bool replace;

        string filterPattern;

        string replacePattern;

        string replaceWith;

        public RegexProcessorViewModel()
        {
            base.BaseSettings.SelectedOrientation = "Vertical";

            // set strings to empty instead of null to prevent null-ref exceptions
            FilterPattern = string.Empty;
            ReplacePattern = string.Empty;
            ReplaceWith = string.Empty;
        }

        public bool Filter
        {
            get { return filter; }
            set { SetProperty(ref filter, value); }
        }

        public bool Replace
        {
            get { return replace; }
            set { SetProperty(ref replace, value); }
        }

        public string FilterPattern
        {
            get { return filterPattern; }
            set { SetProperty(ref filterPattern, value); }
        }

        public string ReplacePattern
        {
            get { return replacePattern; }
            set { SetProperty(ref replacePattern, value); }
        }

        public string ReplaceWith
        {
            get { return replaceWith; }
            set { SetProperty(ref replaceWith, value); }
        }
    }
}
