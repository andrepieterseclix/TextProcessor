using System.ComponentModel.Composition;
using System.IO;
using TextProcessor.Infrastructure.Plugins;
using TextProcessor.Processors.ViewModels;
using TextProcessor.Processors.Views;

namespace TextProcessor.Processors
{
    [Export(typeof(ITextProcessor))]
    class FileNameProcessor : BaseProcessor
    {
        FileNameProcessorViewModel fileProcessorViewModel;

        protected override void init()
        {
            // assign the base view model, but keep a local reference to the more specific type
            base.viewModel = (fileProcessorViewModel = new FileNameProcessorViewModel());
            base.view = new FileNameProcessorView() { DataContext = fileProcessorViewModel };
        }

        protected override bool checkIfExclude(int index, int length, string textItem)
        {
            if (fileProcessorViewModel.ExcludeNonExistingFiles && !File.Exists(textItem))
                return true;
            return base.checkIfExclude(index, length, textItem);
        }

        protected override string processItem(string textItem)
        {
            if (fileProcessorViewModel.SelectedProcess == "File Name")
                textItem = Path.GetFileName(textItem);
            else if (fileProcessorViewModel.SelectedProcess == "Name Without Extension")
                textItem = Path.GetFileNameWithoutExtension(textItem);

            return textItem;
        }
    }
}
