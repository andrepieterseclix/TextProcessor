using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using TextProcessor.Infrastructure;
using TextProcessor.Infrastructure.Plugins;
using TextProcessor.Infrastructure.Services;

namespace TextProcessor.Services
{
    [Export(typeof(IOutputService))]
    class OutputService : IOutputService
    {
        [Import(typeof(ILogService))]
        ILogService logService = null;

        internal string GetOutput(IEnumerable<string> selectedItems, ITextProcessor textProcessor)
        {
            try
            {
                StringBuilder buffer = new StringBuilder();
                int count = selectedItems.Count();
                for (int i = 0; i < count; i++)
                    textProcessor.AppendTextItem(i, count, selectedItems.ElementAt(i), buffer);

                return buffer.ToString();
            }
            catch (Exception ex)
            {
                logService.Write(LogEntryType.Warning, "Error occured while appending text item in '{0}':  {1}", textProcessor.GetType().Name, ex.Message);
                return string.Empty;
            }
        }

        internal Action RefreshAction { get; set; }

        public void Refresh()
        {
            Action refresh = RefreshAction;
            if (refresh != null)
                refresh();
        }
    }
}
