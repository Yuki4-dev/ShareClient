using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace SharedClientForm
{
    public partial class DisplaySelectForm : Form
    {
        private readonly Dictionary<string, IntPtr> windows = new Dictionary<string, IntPtr>();
        public readonly Action<string, IntPtr> SelectedDisplayCallback;

        public DisplaySelectForm(Action<string, IntPtr> callback)
        {
            InitializeComponent();

            SelectedDisplayCallback = callback;

            foreach (Process p in Process.GetProcesses())
            {
                if (p.MainWindowTitle.Length != 0)
                {
                    WindwAdd(p.MainWindowTitle, p.MainWindowHandle);
                }
            }
        }

        private void WindwAdd(string title, IntPtr h)
        {
            if (windows.ContainsKey(title))
            {
                var t = title + "1";
                WindwAdd(t, h);
                return;
            }
            windows.Add(title, h);
            WindowTextList.Items.Add(title);
        }

        private void WindowTextList_SelectedIndexChanged(object sender, EventArgs e)
        {
            var title = WindowTextList.SelectedItem as string;
            var hWnd = windows[title];
            if (BmpHelper.TryGetWindow(hWnd, out Bitmap windowBmp))
            {
                DisplayArea.PaintPicture(windowBmp);
            }
        }

        private void WindowTextList_DoubleClick(object sender, EventArgs e)
        {
            Selected();
        }

        private void SelectBtn_Click(object sender, EventArgs e)
        {
            Selected();
        }

        private void Selected()
        {
            if (WindowTextList.SelectedItem is not string title)
            {
                return;
            }

            var hWnd = windows[title];
            SelectedDisplayCallback.Invoke(title, hWnd);

            Close();
        }
    }
}
