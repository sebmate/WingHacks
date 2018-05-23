using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;
using System.Diagnostics;

namespace WWSwitch
{
    public partial class WWSwitch : Form
    {

        /*
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
        */

        public WWSwitch()
        {
            InitializeComponent();
            TopMost = true;
            
            //var th = new Thread(ExecuteInForeground);
            //th.Start();

            //SetForegroundWindow(this.Handle);

        }

        Form f = new TaskSwitcher();
        private void OpenSwitcher(object sender, EventArgs e)
        {
            f.Show();
        }

        /*
        [DllImport("user32.dll")]
        public static extern void SwitchToThisWindow(IntPtr hWnd);

        private static void ExecuteInForeground()
        {
            while (true)
            {
                System.Diagnostics.Debug.WriteLine("Lalala");
                Process[] processlist = Process.GetProcesses();
                foreach (Process process in processlist)
                {
                    if (!String.IsNullOrEmpty(process.MainWindowTitle))
                    {
                        if (process.MainWindowTitle == "WWSwitcher")
                        {
                            SetForegroundWindow(process.MainWindowHandle);

                            //SwitchToThisWindow(process.MainWindowHandle);
                            //SetWindowPos(process.MainWindowHandle, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW);
                        }
                    }
                }
                
                Thread.Sleep(2000);
            } 
        }
        */
        
    }
}


