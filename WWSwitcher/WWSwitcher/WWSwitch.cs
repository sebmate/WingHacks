using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;
using System.Diagnostics;
using System.Text;

namespace WWSwitch
{
    public partial class WWSwitch : Form
    {

        public WWSwitch()
        {
            InitializeComponent();
            TopMost = true;
            
            var th = new Thread(ExecuteInForeground);
            th.Start();

        }

        Form f = new TaskSwitcher();
        private void OpenSwitcher(object sender, EventArgs e)
        {
            f.Show();
        }

        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        
        [DllImport("user32.dll")]
        public static extern void SwitchToThisWindow(IntPtr hWnd);


        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);


        private static void ExecuteInForeground()
        {
            while (true)
            {
                Process[] processlist = Process.GetProcesses();
                foreach (Process process in processlist)
                {
                    if (!String.IsNullOrEmpty(process.MainWindowTitle))
                    {
                        if (process.MainWindowTitle == "WWSwitcher")
                        {

                            const int nChars = 256;
                            StringBuilder Buff = new StringBuilder(nChars);
                            IntPtr handle = GetForegroundWindow();

                            if (GetWindowText(handle, Buff, nChars) > 0)
                            {
                                //Debug.WriteLine("'" + Buff.ToString() + "'");
                                //if (Buff.ToString() != "WWSwitcher" && Buff.ToString() != "Startmenü" && Buff.ToString() != "TaskSwitcher")
                                if (Buff.ToString() == "WERSI OAS" || Buff.ToString() == "Program Manager")
                                {
                                    Debug.WriteLine("Bringing to front");
                                    SetForegroundWindow(process.MainWindowHandle);
                                }
                            }
                        }
                    }
                }
                Thread.Sleep(1000);
            } 
        }


    }
}


