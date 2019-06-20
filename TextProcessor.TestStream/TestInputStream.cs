using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using TextProcessor.Infrastructure;
using TextProcessor.Infrastructure.Helpers;
using TextProcessor.Infrastructure.Plugins;
using TextProcessor.Infrastructure.Services;

namespace TextProcessor.TestStream
{
    [Export(typeof(IInputStream))]
    public class TestInputStream : IInputStream
    {
        [Import(typeof(ILogService))]
        ILogService logService = null;

        IStreamService streamService;

        CancellationTokenSource cancellationTokenSource;

        Task loopTask;

        RandomTextHelper randomText = new RandomTextHelper();

        public string Name { get { return "Test Stream (Random)"; } }

        public InputStreamState State { get; private set; }

        public void Initialize(IStreamService streamService)
        {
            this.streamService = streamService;
        }

        public void Start()
        {
            cancellationTokenSource = new CancellationTokenSource();
            loopTask = Task.Factory.StartNew(loop, cancellationTokenSource.Token);
            State = InputStreamState.Started;
        }

        public void Stop()
        {
            State = InputStreamState.Stopping;
            cancellationTokenSource.Cancel();
        }

        void loop()
        {
            while (true)
            {
                if (cancellationTokenSource.Token.IsCancellationRequested)
                {
                    DispatcherHelper.Execute(() =>
                    {
                        State = InputStreamState.Stopped;
                        CommandManager.InvalidateRequerySuggested();
                    });
                    return;
                }

                streamService.SendStreamText(randomText.GetRandomString());
                Thread.Sleep(2500);
            }
        }
    }
}
