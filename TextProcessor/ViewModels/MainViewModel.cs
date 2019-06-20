using Framework.Wpf.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using TextProcessor.Infrastructure;
using TextProcessor.Infrastructure.Plugins;
using TextProcessor.Infrastructure.Services;
using TextProcessor.Interfaces;
using TextProcessor.Models;
using TextProcessor.Services;

namespace TextProcessor.ViewModels
{
    class MainViewModel : ViewModelBase
    {
        #region fields

        readonly string manualOrderingDescription = "(manual ordering)";

        IInputStream selectedStream;

        ITextProcessor selectedProcessor;

        ILogService logService;

        IStreamService streamService;

        IDialogService dialogService;

        OutputService outputService;

        string orderByTextItem;

        int warningCounter;

        int errorCounter;

        bool isIdle;

        string output;

        bool loadCompleted;

        string streamState;

        #endregion

        #region ctor

        public MainViewModel(IEnumerable<IInputStream> streams, IEnumerable<ITextProcessor> processors, ILogService logger, IStreamService streamer, IOutputService outputHandler, IDialogService dialogHandler)
        {
            // fields and properties
            Streams = new ObservableCollection<IInputStream>(streams);
            Processors = new ObservableCollection<ITextProcessor>(processors);
            OrderByTextItemList = new string[] { manualOrderingDescription, "TimeStamp", "Text" };
            StreamedItems = new ObservableCollection<TextItemViewModel>();
            StreamedItemsView = new ListCollectionView(StreamedItems);
            outputService = outputHandler as OutputService;
            dialogService = dialogHandler;
            logService = logger;
            streamService = streamer;
            OrderByTextItem = OrderByTextItemList.FirstOrDefault();
            SelectedStream = Streams.FirstOrDefault();
            SelectedProcessor = Processors.FirstOrDefault();
            StreamedItems.CollectionChanged += (s, e) => updateOutput();

            #region initialize services internal functions

            // set up the logger for binding
            IObservableLogger loggerWithEntries = null;
            if (logService != null && (loggerWithEntries = logService as IObservableLogger) != null)
            {
                LogEntries = new ListCollectionView(loggerWithEntries.Entries);
                loggerWithEntries.Entries.CollectionChanged += (s, e) => refreshLogCounters();
                refreshLogCounters();
            }

            // set the callback for the stream service
            (streamService as StreamService).ReceiveStreamText = streamedText =>
            {
                StreamedItems.Add(new TextItemViewModel(streamedText, outputService));
                CommandManager.InvalidateRequerySuggested();

                // NOTE:  updateOutput is called on the CollectionChanged event of StreamedItems
            };

            // set up output service
            outputService.RefreshAction = updateOutput;

            #endregion

            // commands
            StartCommand = CreateCommand(startCommand_Execute, startCommand_CanExecute);
            StopCommand = CreateCommand(stopCommand_Execute, stopCommand_CanExecute);
            SelectAllCommand = CreateCommand(selectAllCommand_Execute, selectAllCommand_CanExecute);
            DeselectAllCommand = CreateCommand(deselectAllCommand_Execute, deselectAllCommand_CanExecute);
            DeleteCommand = CreateCommand(deleteCommand_Execute, deleteCommand_CanExecute);
            MoveUpCommand = CreateCommand(moveUpCommand_Execute, moveUpCommand_CanExecute);
            MoveDownCommand = CreateCommand(moveDownCommand_Execute, moveDownCommand_CanExecute);
            CopyOutputCommand = CreateCommand(copyOutputCommand_Execute, copyOutputCommand_CanExecute);
            HelpCommand = CreateCommand(helpCommand_Execute);

            // enable the control used to select an input stream
            IsIdle = true;
            loadCompleted = true;
            updateOutput();
            refreshStreamStateLabel();
        }

        #endregion

        #region properties

        public ICommand StartCommand { get; private set; }

        public ICommand StopCommand { get; private set; }

        public ICommand SelectAllCommand { get; private set; }

        public ICommand DeselectAllCommand { get; private set; }

        public ICommand DeleteCommand { get; private set; }

        public ICommand MoveUpCommand { get; private set; }

        public ICommand MoveDownCommand { get; private set; }

        public ICommand CopyOutputCommand { get; private set; }

        public ICommand HelpCommand { get; private set; }

        public string[] OrderByTextItemList { get; private set; }

        public ListCollectionView StreamedItemsView { get; private set; }

        public ObservableCollection<TextItemViewModel> StreamedItems { get; private set; }

        public ObservableCollection<IInputStream> Streams { get; private set; }

        public ObservableCollection<ITextProcessor> Processors { get; private set; }

        public ListCollectionView LogEntries { get; private set; }

        public IInputStream SelectedStream
        {
            get { return selectedStream; }
            set
            {
                if (SetProperty(ref selectedStream, value))
                    updateOutput();
            }
        }

        public ITextProcessor SelectedProcessor
        {
            get { return selectedProcessor; }
            set
            {
                if (SetProperty(ref selectedProcessor, value))
                    updateOutput();
            }
        }

        public string OrderByTextItem
        {
            get { return orderByTextItem; }
            set
            {
                if (SetProperty(ref orderByTextItem, value))
                {
                    updateItemsOrder();
                    updateOutput();
                }
            }
        }

        public int WarningCounter
        {
            get { return warningCounter; }
            set { SetProperty(ref warningCounter, value); }
        }

        public int ErrorCounter
        {
            get { return errorCounter; }
            set { SetProperty(ref errorCounter, value); }
        }

        public bool IsIdle
        {
            get { return isIdle; }
            set { SetProperty(ref isIdle, value); }
        }

        public string Output
        {
            get { return output; }
            set { SetProperty(ref output, value); }
        }

        public string StreamState
        {
            get { return streamState; }
            set { SetProperty(ref streamState, value); }
        }

        #endregion

        #region methods

        void updateItemsOrder()
        {
            StreamedItemsView.SortDescriptions.Clear();
            if (string.Equals(OrderByTextItem, manualOrderingDescription))
                return;

            StreamedItemsView.SortDescriptions.Add(new SortDescription(OrderByTextItem, ListSortDirection.Ascending));
        }

        void refreshLogCounters()
        {
            var obs = LogEntries.SourceCollection as ObservableCollection<LogEntry>;
            if (obs == null)
                return;

            WarningCounter = obs.Count(le => le.EntryType == LogEntryType.Warning);
            errorCounter = obs.Count(le => le.EntryType == LogEntryType.Error);
        }

        void refreshStreamStateLabel()
        {
            if (SelectedStream == null)
            {
                StreamState = string.Empty;
                return;
            }

            StreamState = SelectedStream.State.ToString();
        }

        void updateOutput()
        {
            try
            {
                if (!loadCompleted || SelectedProcessor == null)
                {
                    Output = null;
                    return;
                }

                var selectedItems = StreamedItemsView.Cast<TextItemViewModel>().Where(si => si.IsSelected).Select(si => si.Text).ToArray();

                // do the work in a task to prevent the UI from freezing
                Task.Factory.StartNew(() =>
                {
                    string outputText = outputService.GetOutput(selectedItems, SelectedProcessor);
                    Invoke(() => Output = outputText);
                });
            }
            catch (Exception ex)
            {
                Output = null;
                var procName = SelectedProcessor == null ? "[No Processor Selected]" : SelectedProcessor.Name;
                logService.Write("Failed to build the output with processor '{0}':  {1}", procName, ex.Message);
            }
        }

        void startCommand_Execute(object parameter)
        {
            try
            {
                IsIdle = false;
                SelectedStream.Start();
                logService.Write("Started streaming from '{0}'", SelectedStream.Name);
            }
            catch (Exception ex)
            {
                logService.Write(LogEntryType.Error, "Could not start input stream '{0}':  {1}", SelectedStream.Name, ex.Message);
                IsIdle = true;
            }
            finally
            {
                refreshStreamStateLabel();
            }
        }

        bool startCommand_CanExecute(object parameter)
        {
            bool canStart = SelectedStream == null ? false : SelectedStream.State == InputStreamState.Stopped;
            if (canStart)
                IsIdle = true;
            refreshStreamStateLabel();
            return canStart;
        }

        void stopCommand_Execute(object parameter)
        {
            try
            {
                SelectedStream.Stop();
                logService.Write("Stopped streaming from '{0}'", SelectedStream.Name);
                // IsIdle is set in the startCommand_CanExecute method which is invoked when the InputStreamState changes to Stopped
            }
            catch (Exception ex)
            {
                logService.Write(LogEntryType.Error, "Could not stop input stream '{0}':  {1}", SelectedStream.Name, ex.Message);
            }
            finally
            {
                refreshStreamStateLabel();
            }
        }

        bool stopCommand_CanExecute(object parameter)
        {
            return SelectedStream == null ? false : SelectedStream.State == InputStreamState.Started;
        }

        void selectAllCommand_Execute(object parameter)
        {
            foreach (var streamedItem in StreamedItems)
                streamedItem.IsSelected = true;
        }

        bool selectAllCommand_CanExecute(object parameter)
        {
            return StreamedItems.Any(si => !si.IsSelected);
        }

        void deselectAllCommand_Execute(object parameter)
        {
            foreach (var streamedItem in StreamedItems)
                streamedItem.IsSelected = false;
        }

        bool deselectAllCommand_CanExecute(object parameter)
        {
            return StreamedItems.Any(si => si.IsSelected);
        }

        void deleteCommand_Execute(object parameter)
        {
            var highlighted = StreamedItems.Where(si => si.IsHighlighted).ToList();
            foreach (var removeItem in highlighted)
                StreamedItems.Remove(removeItem);
        }

        bool deleteCommand_CanExecute(object parameter)
        {
            return StreamedItems.Count(si => si.IsHighlighted) > 0;
        }

        void moveUpCommand_Execute(object parameter)
        {
            var highlighted = StreamedItems.Where(si => si.IsHighlighted).ToList();
            StreamedItems.ToList().ForEach(h => h.IsHighlighted = false);
            int index;
            foreach (var moveUpItem in highlighted)
            {
                index = StreamedItems.IndexOf(moveUpItem);
                StreamedItems.Move(index, --index);
            }
            highlighted.ForEach(h => h.IsHighlighted = true);
        }

        bool moveUpCommand_CanExecute(object parameter)
        {
            var highlighted = StreamedItems.FirstOrDefault(si => si.IsHighlighted);
            return string.Equals(OrderByTextItem, manualOrderingDescription) && (highlighted == null ? false : StreamedItems.IndexOf(highlighted) > 0);
        }

        void moveDownCommand_Execute(object parameter)
        {
            var highlighted = StreamedItems.Where(si => si.IsHighlighted).ToList();
            StreamedItems.ToList().ForEach(h => h.IsHighlighted = false);
            int index;
            for (var i = highlighted.Count - 1; i >= 0; i--)
            {
                index = StreamedItems.IndexOf(highlighted[i]);
                StreamedItems.Move(index, ++index);
            }
            highlighted.ForEach(h => h.IsHighlighted = true);
        }

        bool moveDownCommand_CanExecute(object parameter)
        {
            var highlighted = StreamedItems.LastOrDefault(si => si.IsHighlighted);
            return string.Equals(OrderByTextItem, manualOrderingDescription) && (highlighted == null ? false : StreamedItems.IndexOf(highlighted) < StreamedItems.Count - 1);
        }

        void copyOutputCommand_Execute(object parameter)
        {
            if (SelectedStream != null && SelectedStream.State != InputStreamState.Stopped)
                if (!dialogService.AskQuestion("Copy Output", "Copying text to the clipboard can affect the output text if the current stream is active.  Do you want to continue?"))
                    return;
            Clipboard.SetText(Output);
        }

        bool copyOutputCommand_CanExecute(object parameter)
        {
            return !string.IsNullOrWhiteSpace(Output);
        }

        void helpCommand_Execute(object parameter)
        {
            dialogService.ShowHelpWindow(false);
        }

        #endregion
    }
}
