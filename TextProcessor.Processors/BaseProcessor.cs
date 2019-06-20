using System;
using System.ComponentModel.Composition;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using TextProcessor.Infrastructure;
using TextProcessor.Infrastructure.Plugins;
using TextProcessor.Infrastructure.Services;
using TextProcessor.Processors.ViewModels;
using TextProcessor.Processors.Views;

namespace TextProcessor.Processors
{
    [Export(typeof(ITextProcessor))]
    class BaseProcessor : ITextProcessor
    {
        bool printedFirstItem;

        [Import(typeof(IOutputService))]
        protected IOutputService outputService = null;

        protected BaseProcessorViewModel viewModel;

        protected FrameworkElement view;

        public string Name
        {
            get
            {
                string name = GetType().Name.Replace("Processor", "");
                return Regex.Replace(name, @"([a-z])([A-Z])", @"$1 $2");
            }
        }


        public FrameworkElement View { get { return view; } }

        public void Initialize()
        {
            init();

            // if a property changes on the view model, trigger the output to update
            if (outputService != null)
            {
                viewModel.PropertyChanged += (s, e) => outputService.Refresh();
                viewModel.BaseSettings.PropertyChanged += (s, e) => outputService.Refresh();
            }
        }

        protected virtual void init()
        {
            viewModel = new BaseProcessorViewModel();
            view = new BaseProcessorView() { DataContext = viewModel };
        }

        public void AppendTextItem(int index, int length, string textItem, StringBuilder buffer)
        {
            // printedFirstItem is used to prevent printing the separator or a new line for vertical orientation if the first item is excluded
            if (index == 0)
                printedFirstItem = false;

            // derived types might want to exclude certain items
            if (checkIfExclude(index, length, textItem))
                return;

            // separator
            if (printedFirstItem)
                buffer.Append(viewModel.BaseSettings.Separator);

            // orientation
            if (viewModel.BaseSettings.SelectedOrientation == "Vertical" && printedFirstItem)
                buffer.AppendLine();

            // print text item
            buffer.AppendFormat("{1}{0}{2}", processItem(textItem), viewModel.BaseSettings.GetFrontEnclosingString(), viewModel.BaseSettings.GetBackEnclosingString());
            if (!printedFirstItem)
                printedFirstItem = true;
        }

        protected virtual bool checkIfExclude(int index, int length, string textItem)
        {
            return false;
        }

        protected virtual string processItem(string textItem)
        {
            return textItem;
        }
    }
}
