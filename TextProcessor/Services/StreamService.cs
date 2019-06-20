using System;
using System.ComponentModel.Composition;
using TextProcessor.Infrastructure.Helpers;
using TextProcessor.Infrastructure.Services;

namespace TextProcessor.Services
{
    [Export(typeof(IStreamService))]
    class StreamService : IStreamService
    {
        public void SendStreamText(string text)
        {
            var callback = ReceiveStreamText;
            if (callback != null)
                DispatcherHelper.Execute(() => callback(text));
        }

        internal Action<string> ReceiveStreamText { get; set; }
    }
}
