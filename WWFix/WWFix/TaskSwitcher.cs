using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace WWSwitch
{
    public partial class TaskSwitcher : Form
    {
        public TaskSwitcher()
        {
            InitializeComponent();
        }

        [DllImport("user32.dll")]
        public static extern void SwitchToThisWindow(IntPtr hWnd);

        [DllImport("User32")]
        private static extern int keybd_event(Byte bVk, Byte bScan, long dwFlags, long dwExtraInfo);
        private const byte UP = 2;
        private const byte CTRL = 17;
        private const byte ESC = 27;

        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        private const UInt32 SWP_NOSIZE = 0x0001;
        private const UInt32 SWP_NOMOVE = 0x0002;
        private const UInt32 TOPMOST_FLAGS = SWP_NOMOVE | SWP_NOSIZE;

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
        
        private void TaskSwitcher_Load(object sender, EventArgs e)
        {
            SetWindowPos(this.Handle, HWND_TOPMOST, 0, 0, 0, 0, TOPMOST_FLAGS);
            refreshTasklist();
        }

        private void refreshTasklist()
        {
            button1.Text = "";
            button1.Hide();
            button2.Text = "";
            button2.Hide();
            button3.Text = "";
            button3.Hide();
            button4.Text = "";
            button4.Hide();
            button5.Text = "";
            button5.Hide();
            button6.Text = "";
            button6.Hide();

            Process[] processlist = Process.GetProcesses();

            int atProcess = 0;
            foreach (Process process in processlist)
            {

                if (!String.IsNullOrEmpty(process.MainWindowTitle))
                {

                    if (!process.MainWindowTitle.Contains("vb3w.dll") &&  process.MainWindowTitle != "WERSI OAS" && process.MainWindowTitle != "WWSwitcher" && process.MainWindowTitle != "TaskSwitcher" && process.MainWindowTitle != "PenMount Gesture AP" && process.MainWindowTitle != "Program Manager")
                    {
                        atProcess = atProcess + 1;
                        switch (atProcess)
                        {
                            case 1:
                                button1.Text = process.MainWindowTitle;
                                button1.Show();
                                break;
                            case 2:
                                button2.Text = process.MainWindowTitle;
                                button2.Show();
                                break;
                            case 3:
                                button3.Text = process.MainWindowTitle;
                                button3.Show();
                                break;
                            case 4:
                                button4.Text = process.MainWindowTitle;
                                button4.Show();
                                break;
                            case 5:
                                button5.Text = process.MainWindowTitle;
                                button5.Show();
                                break;
                            case 6:
                                button6.Text = process.MainWindowTitle;
                                button6.Show();
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            System.Environment.Exit(1);
        }

        private void TaskSwitcher_Deactivate(object sender, EventArgs e)
        {
            Hide();
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            // Press Ctrl-Esc key to open Start menu
            keybd_event(CTRL, 0, 0, 0);
            keybd_event(ESC, 0, 0, 0);

            // Need to Release those two keys
            keybd_event(CTRL, 0, UP, 0);
            keybd_event(ESC, 0, UP, 0);
        }

        private void switchToTask(String task)
        {
            Process[] processlist = Process.GetProcesses();
            foreach (Process process in processlist)
            {
                if (!String.IsNullOrEmpty(process.MainWindowTitle))
                {
                    if (process.MainWindowTitle == task)
                    {
                        SwitchToThisWindow(process.MainWindowHandle);
                    }
                }
            }

        }

        private void switchToOAS_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            switchToTask(button1.Text);
        }
        
        private void button2_Click(object sender, EventArgs e)
        {
            switchToTask(button2.Text);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            switchToTask(button3.Text);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            switchToTask(button4.Text);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            switchToTask(button5.Text);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            switchToTask(button6.Text);
        }

        private void TaskSwitcher_Shown(object sender, EventArgs e)
        {
            refreshTasklist();
        }

        private void TaskSwitcher_Activated(object sender, EventArgs e)
        {
            refreshTasklist();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
