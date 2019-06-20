using System.Text;
using System.Windows;

namespace TextProcessor.Infrastructure.Plugins
{
    public interface ITextProcessor
    {
        string Name { get; }

        FrameworkElement View { get; }

        void Initialize();

        void AppendTextItem(int index, int length, string textItem, StringBuilder buffer);
    }
}
