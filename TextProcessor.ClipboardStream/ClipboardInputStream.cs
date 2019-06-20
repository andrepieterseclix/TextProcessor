using System.ComponentModel.Composition;
using TextProcessor.Infrastructure;
using TextProcessor.Infrastructure.Plugins;
using TextProcessor.Infrastructure.Services;

namespace TextProcessor.ClipboardStream
{
    [Export(typeof(IInputStream))]
    class ClipboardInputStream : IInputStream
    {
        ClipboardListenerForm form = null;

        public string Name { get { return "Clipboard Stream"; } }

        public InputStreamState State { get; private set; }

        public void Initialize(IStreamService streamService)
        {
            form = new ClipboardListenerForm();
            form.StreamService = streamService;
        }

        public void Start()
        {
            form.Start();
            State = InputStreamState.Started;
        }

        public void Stop()
        {
            form.Stop();
            State = InputStreamState.Stopped;
        }
    }
}
