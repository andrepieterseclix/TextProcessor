using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using TextProcessor.Infrastructure.Services;

namespace TextProcessor.ClipboardStream
{
    [Export(typeof(ClipboardListenerForm))]
    internal partial class ClipboardListenerForm : Form
    {
        IntPtr nextClipboardViewer;

        string currentTextContent;

        string currentFilesHashCode;

        bool started;

        public ClipboardListenerForm()
        {
            InitializeComponent();
        }

        internal IStreamService StreamService { get; set; }

        [DllImport("User32.dll")]
        protected static extern int SetClipboardViewer(int hWndNewViewer);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern bool ChangeClipboardChain(IntPtr hWndRemove, IntPtr hWndNewNext);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);

        private void handleClipboardEvent()
        {
            IDataObject data = Clipboard.GetDataObject();

            if (data.GetDataPresent(DataFormats.FileDrop))
            {
                // order the array, because the hash will be different if the same files are copied but in different order (it depends on over which file the mouse cursor is sometimes)
                var paths = ((string[])data.GetData(DataFormats.FileDrop)).OrderBy(s => s).ToArray();

                // calculate the hash value of the selected files to compare with previously copied files
                string hashValue = HashHelper.CalculateHashString(paths);
                if (!string.IsNullOrWhiteSpace(currentFilesHashCode) && currentFilesHashCode.Equals(hashValue))
                    return;

                currentFilesHashCode = hashValue;
                foreach (var fileOrDirectory in paths)
                    StreamService.SendStreamText(fileOrDirectory);
            }
            else if (data.GetDataPresent(DataFormats.Text))
            {
                string clipboardText = (string)data.GetData(DataFormats.Text);

                // when copying text from pdf, null text are picked up, and this event is raised multiple times
                if (string.IsNullOrEmpty(clipboardText) || clipboardText.Equals(currentTextContent))
                    return;

                currentTextContent = clipboardText;

                // if the clipboard text spans multiple lines, split text items into individual lines
                string[] lines = Regex.Split(clipboardText, "\r\n");
                foreach (var line in lines)
                    if (!string.IsNullOrWhiteSpace(line))
                        StreamService.SendStreamText(line);
            }
        }

        protected override void WndProc(ref Message m)
        {
            // defined in winuser.h
            const int WM_DRAWCLIPBOARD = 0x308;
            const int WM_CHANGECBCHAIN = 0x030D;

            switch (m.Msg)
            {
                case WM_DRAWCLIPBOARD:
                    if (started)
                        handleClipboardEvent();
                    SendMessage(nextClipboardViewer, m.Msg, m.WParam, m.LParam);
                    break;
                case WM_CHANGECBCHAIN:
                    if (m.WParam == nextClipboardViewer)
                        nextClipboardViewer = m.LParam;
                    else
                        SendMessage(nextClipboardViewer, m.Msg, m.WParam, m.LParam);
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        internal void Start()
        {
            if (started)
                return;

            nextClipboardViewer = (IntPtr)SetClipboardViewer((int)this.Handle);
            started = true;
        }

        internal void Stop()
        {
            if (!started)
                return;

            ChangeClipboardChain(this.Handle, nextClipboardViewer);
            started = false;
        }
    }
}
