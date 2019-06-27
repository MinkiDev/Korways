using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace Korways
{
    public partial class Korways : Form
    {
        public Korways()
        {
            InitializeComponent();
            Thread imeHandler = new Thread(new ThreadStart(GetIME));
            imeHandler.Start();
            MessageBox.Show("* Service has been started *\nTo stop, just kill process \"" +
                Process.GetCurrentProcess().ProcessName + ".exe\"",
                "Korways IME Fixer", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }

        void GetIME()
        {
            while (true)
            {

                Process p = GetProcessById(GetActiveProcess());

                if (p == null)
                    return;

                IntPtr hwnd = p.MainWindowHandle;
                IntPtr hime = ImmGetDefaultIMEWnd(hwnd);
                IntPtr status = SendMessage(hime, WM_IME_CONTROL, new IntPtr(0x5), new IntPtr(0));

                if (status.ToInt32() != 0)
                {
                    // Hangul Mode
                }
                else
                {
                    // English Mode
                    keybd_event((byte)Keys.HangulMode, 0, 0x00, 0);
                    keybd_event((byte)Keys.HangulMode, 0, 0x02, 0);
                }
                Thread.Sleep(10);
            }
        }

        public Process GetProcessById(int id)
        {
            Process[] processlist = Process.GetProcesses();
            return processlist.FirstOrDefault(pr => pr.Id == id);
        }

        // Returns the id of the process owning the foreground window.
        private int GetActiveProcess()
        {
            IntPtr hwnd = GetForegroundWindow();

            // The foreground window can be NULL in certain circumstances, 
            // such as when a window is losing activation.
            if (hwnd == null)
                return 0;

            uint pid;
            GetWindowThreadProcessId(hwnd, out pid);

            foreach (System.Diagnostics.Process p in System.Diagnostics.Process.GetProcesses())
            {
                if (p.Id == pid)
                    return (int)pid;
            }

            return 0;
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern Int32 GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("imm32.dll")]
        private static extern IntPtr ImmGetDefaultIMEWnd(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr IParam);

        private const int WM_IME_CONTROL = 643;

        [DllImport("user32.dll")]
        public static extern void keybd_event(uint vk, uint scan, uint flags, uint extraInfo);

        [DllImport("user32.dll")]
        private static extern uint MapVirtualKey(int wCode, int wMapType);

        private void Korways_Shown(object sender, EventArgs e)
        {
            Hide();
        }
    }
}
