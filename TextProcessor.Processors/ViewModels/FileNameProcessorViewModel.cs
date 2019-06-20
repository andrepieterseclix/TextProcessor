using System.Linq;

namespace TextProcessor.Processors.ViewModels
{
    class FileNameProcessorViewModel : BaseProcessorViewModel
    {
        bool excludeNonExistingFiles;

        string selectedProcess;

        public FileNameProcessorViewModel()
        {
            Processes = new string[] { "Full Path", "File Name", "Name Without Extension" };
            SelectedProcess = Processes.FirstOrDefault();
            base.BaseSettings.SelectedOrientation = "Vertical";
        }

        public bool ExcludeNonExistingFiles
        {
            get { return excludeNonExistingFiles; }
            set { SetProperty(ref excludeNonExistingFiles, value); }
        }

        public string[] Processes { get; private set; }

        public string SelectedProcess
        {
            get { return selectedProcess; }
            set { SetProperty(ref selectedProcess, value); }
        }
    }
}
