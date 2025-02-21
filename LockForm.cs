using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace DesktopLockApp
{
    public partial class LockForm : Form
    {
        private const string validPassword = "1234";

        [DllImport("user32.dll")]
        public static extern int SetWindowsHookEx(int idHook, LowLevelProc lpfn, IntPtr hInstance, uint threadId);

        [DllImport("user32.dll")]
        public static extern int UnhookWindowsHookEx(int idHook);

        [DllImport("user32.dll")]
        public static extern int CallNextHookEx(int idHook, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        private const int WH_KEYBOARD_LL = 13;
        private const int WH_MOUSE_LL = 14;

        private static int _keyboardHookID = 0;
        private static int _mouseHookID = 0;

        private bool isUnlockScreenVisible = false;

        public delegate int LowLevelProc(int nCode, IntPtr wParam, IntPtr lParam);

        public LockForm()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            this.TopMost = true;
            this.BackColor = Color.Black; // Removed System.Drawing namespace
            this.Opacity = 1.0;
            this.StartPosition = FormStartPosition.Manual;
            Rectangle screenArea = GetAllScreensArea();
            this.Bounds = screenArea;

            _keyboardHookID = SetWindowsHookEx(WH_KEYBOARD_LL, KeyboardHookCallback, GetModuleHandle("user32.dll"), 0);
            _mouseHookID = SetWindowsHookEx(WH_MOUSE_LL, MouseHookCallback, GetModuleHandle("user32.dll"), 0);

            Application.AddMessageFilter(new KeyPressMessageFilter(this));
        }

        private Rectangle GetAllScreensArea()
        {
            int totalWidth = 0;
            int totalHeight = 0;
            foreach (Screen screen in Screen.AllScreens)
            {
                totalWidth += screen.Bounds.Width;
                totalHeight = Math.Max(totalHeight, screen.Bounds.Height);
            }
            return new Rectangle(0, 0, totalWidth, totalHeight);
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct KBDLLHOOKSTRUCT
        {
            public int vkCode;
            public int scanCode;
            public int flags;
            public int time;
            public IntPtr dwExtraInfo;
        }

        private int KeyboardHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                KBDLLHOOKSTRUCT keyInfo = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(KBDLLHOOKSTRUCT));
                if (keyInfo.vkCode == (int)Keys.L) // Allow 'L' key to pass through
                {
                    return CallNextHookEx(_keyboardHookID, nCode, wParam, lParam);
                }
                if (!isUnlockScreenVisible)
                {
                    return 1; // Block all other keys
                }
            }
            return CallNextHookEx(_keyboardHookID, nCode, wParam, lParam);
        }

        private int MouseHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (!isUnlockScreenVisible)
            {
                return 1; // Block mouse input
            }
            return CallNextHookEx(_mouseHookID, nCode, wParam, lParam);
        }

        private class KeyPressMessageFilter : IMessageFilter
        {
            private readonly LockForm _lockForm;

            public KeyPressMessageFilter(LockForm lockForm)
            {
                _lockForm = lockForm;
            }

            public bool PreFilterMessage(ref Message m)
            {
                if (m.Msg == 0x100 && m.WParam.ToInt32() == (int)Keys.L)
                {
                    _lockForm.ShowPasswordPrompt();
                    return true;
                }
                return false;
            }
        }

        private void ShowPasswordPrompt()
        {
            isUnlockScreenVisible = true;
            using (var prompt = new PasswordPromptForm())
            {
                var result = prompt.ShowDialog();
                if (result == DialogResult.OK)
                {
                    if (prompt.Password == validPassword)
                    {
                        MessageBox.Show("Unlock Successful!");
                        UnlockInput();
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Incorrect Password!");
                    }
                }
            }
            isUnlockScreenVisible = false;
        }

        private void UnlockInput()
        {
            UnhookWindowsHookEx(_keyboardHookID);
            UnhookWindowsHookEx(_mouseHookID);
        }

        private string PromptForPassword()
        {
            using (var prompt = new PasswordPromptForm())
            {
                var result = prompt.ShowDialog();
                if (result == DialogResult.OK)
                {
                    return prompt.Password;
                }
                return string.Empty;
            }
        }
    }
}