using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace TG.Common
{
    public partial class WaitForm : Form
    {
        private WaitForm()
        {
            InitializeComponent();
        }

        static WaitForm Instance { get; set; }

        static string _message;
        static bool _placeParent;
        static Form _parent;
        static IntPtr _parentHandle;
        static int _parentRight, _parentBottom, _parentWidth, _parentHeight;
        static Thread _winThread;
        static int _timeout;

        /// <summary>
        /// Shows the WaitForm with a default message of "Waiting".
        /// </summary>
        public static void ShowForm()
        {
            ShowForm("Waiting", null, -1);
        }

        /// <summary>
        /// Show the WaitForm or updates the message.
        /// </summary>
        /// <param name="message">The message to display in the WaitForm.</param>
        public static void ShowForm(string message)
        {
            ShowForm(message, null, -1);
        }

        /// <summary>
        /// Show the WaitForm or updates the message.
        /// </summary>
        /// <param name="message">The message to display in the WaitForm.</param>
        /// <param name="parent">The parent of the WaitForm.</param>
        public static void ShowForm(string message, Form parent)
        {
            ShowForm(message, parent, -1);
        }

        /// <summary>
        /// Show the WaitForm or updates the message.
        /// </summary>
        /// <param name="message">The message to display in the WaitForm.</param>
        /// <param name="parent">The parent of the WaitForm.</param>
        /// <param name="autoCloseTimeout">The delay, in milliseconds, when the form should close automatically.</param>
        public static void ShowForm(string message, Form parent, int autoCloseTimeout)
        {
            if (_winThread == null)
            {
                _message = message;
                if (parent != null)
                {
                    _parent = parent;
                    _parentHandle = parent.Handle;
                    _placeParent = true;
                    _parentBottom = _parent.Bottom;
                    _parentRight = _parent.Right;
                    _parentHeight = _parent.Height;
                    _parentWidth = _parent.Width;
                    parent.FormClosing += ParentForm_FormClosing;
                }
                _timeout = autoCloseTimeout;

                _winThread = new Thread(new ThreadStart(CreateWindowThread));
                _winThread.Start();
            }
        }

        private static void CreateWindowThread()
        {
            Instance = new WaitForm();

            Instance.lblMessage.Text = _message;

            if (_timeout > -1)
            {
                Instance.AutoCloseTimer.Interval = _timeout;
                Instance.AutoCloseTimer.Start();
            }

            if (_placeParent)
            {

                Size expectedSize = new Size(375, 100);
                EnableWindow(_parentHandle, false);
                Instance.StartPosition = FormStartPosition.Manual;
                Instance.Left = (_parentRight - (_parentWidth / 2)) - (expectedSize.Width / 2);
                Instance.Top = (_parentBottom - (_parentHeight / 2)) - (expectedSize.Height / 2);
                Instance.Size = expectedSize;
                Instance.ShowDialog();
            }
            else
            {
                Instance.StartPosition = FormStartPosition.CenterScreen;
                Instance.ShowDialog();
            }
        }

        private static void ParentForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            CloseForm();
        }

        private static void KillThread()
        {
            if (_placeParent)
            {
                EnableWindow(_parentHandle, true);
            }
            Instance?.Dispose();
            Instance = null;
            _winThread = null;
        }

        private delegate void VoidAction();

        /// <summary>
        /// Closes the WaitForm.
        /// </summary>
        public static void CloseForm()
        {
            if (Instance != null)
            {
                if (Instance.InvokeRequired)
                {
                    Instance.Invoke(new VoidAction(KillThread));
                }
                else
                {
                    KillThread();
                }
            }
        }

        private void AutoClose_Tick(object sender, EventArgs e)
        {
            KillThread();
        }

        //Import the EnableWindow method
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool EnableWindow(IntPtr hWnd, bool bEnable);

        
    }
}
