using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using TextProcessor.Infrastructure;
using TextProcessor.Infrastructure.Plugins;
using TextProcessor.Infrastructure.Services;
using System.Linq;

namespace TextProcessor.Services
{
    [Export(typeof(PluginsService))]
    class PluginsService
    {
        [Import(typeof(ILogService))]
        ILogService logService = null;

        [Import(typeof(IStreamService))]
        IStreamService streamService = null;

        [ImportMany(typeof(IInputStream))]
        internal List<IInputStream> InputStreams { get; private set; }

        [ImportMany(typeof(ITextProcessor))]
        internal List<ITextProcessor> TextProcessors { get; private set; }

        internal void InitializeStreams()
        {
            logService.Write(InputStreams.Count > 0 ? LogEntryType.Info : LogEntryType.Warning, "Found {0} input stream{1}...", InputStreams.Count, InputStreams.Count == 1 ? "" : "s");

            // iterate from the back to be able to remove a stream if it fails to initialize
            for (int i = InputStreams.Count - 1; i >= 0; i--)
            {
                try
                {
                    InputStreams[i].Initialize(streamService);
                    logService.Write("'{0}' initialized", InputStreams[i].Name);
                }
                catch (Exception ex)
                {
                    logService.Write(LogEntryType.Error, "Initializing input stream '{0}' failed:  {1}", InputStreams[i].Name, ex.Message);
                    InputStreams.Remove(InputStreams[i]);
                }
            }

            // order by name
            InputStreams = InputStreams.OrderBy(i => i.Name).ToList();
        }

        internal void InitializeProcessors()
        {
            logService.Write(TextProcessors.Count > 0 ? LogEntryType.Info : LogEntryType.Warning, "Found {0} text processor{1}...", TextProcessors.Count, TextProcessors.Count == 1 ? "" : "s");

            // iterate from the back to be able to remove a processor if it fails to initialize
            for (int i = TextProcessors.Count - 1; i >= 0; i--)
            {
                try
                {
                    TextProcessors[i].Initialize();
                    logService.Write("'{0}' initialized", TextProcessors[i].GetType().Name);
                }
                catch (Exception ex)
                {
                    logService.Write(LogEntryType.Error, "Initializing text processor '{0}' failed:  {1}", TextProcessors[i].GetType().Name, ex.Message);
                    TextProcessors.Remove(TextProcessors[i]);
                }

                // order by name
                TextProcessors = TextProcessors.OrderBy(t => t.Name).ToList();
            }
        }
    }
}
