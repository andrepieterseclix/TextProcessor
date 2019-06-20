using Framework.Wpf.Mvvm;

namespace TextProcessor.Processors.ViewModels
{
    class BaseProcessorViewModel : ViewModelBase
    {
        double firstColumnWidth;

        public BaseProcessorViewModel()
        {
            BaseSettings = new BaseSettingsViewModel();
            FirstColumnWidth = 90d;
        }

        public BaseSettingsViewModel BaseSettings { get; private set; }

        public double FirstColumnWidth
        {
            get { return firstColumnWidth; }
            set { SetProperty(ref firstColumnWidth, value); }
        }
    }
}
