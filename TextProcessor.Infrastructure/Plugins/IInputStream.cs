using TextProcessor.Infrastructure.Services;

namespace TextProcessor.Infrastructure.Plugins
{
    public interface IInputStream
    {
        string Name { get; }

        InputStreamState State { get; }

        void Initialize(IStreamService streamService);

        void Start();

        void Stop();
    }
}
