using System.ComponentModel.Composition;
using System.Text.RegularExpressions;
using TextProcessor.Infrastructure.Plugins;
using TextProcessor.Processors.ViewModels;
using TextProcessor.Processors.Views;

namespace TextProcessor.Processors
{
    [Export(typeof(ITextProcessor))]
    class RegexProcessor : BaseProcessor
    {
        RegexProcessorViewModel regexProcessorViewModel;

        protected override void init()
        {
            // assign the base view model, but keep a local reference to the more specific type
            base.viewModel = (regexProcessorViewModel = new RegexProcessorViewModel());
            base.view = new RegexProcessorView() { DataContext = regexProcessorViewModel };
        }

        protected override bool checkIfExclude(int index, int length, string textItem)
        {
            if (!regexProcessorViewModel.Filter)
                return false;

            return !Regex.IsMatch(textItem, regexProcessorViewModel.FilterPattern);
        }

        protected override string processItem(string textItem)
        {
            if (!regexProcessorViewModel.Replace)
                return base.processItem(textItem);

            return Regex.Replace(textItem, regexProcessorViewModel.ReplacePattern, regexProcessorViewModel.ReplaceWith);
        }
    }
}
