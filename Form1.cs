using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace DesktopLockApp
{
    public partial class Form1 : Form
    {
        // Import LockWorkStation function from user32.dll
        [DllImport("user32.dll")]
        public static extern bool LockWorkStation();

        // Import RegisterHotKey function from user32.dll
        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        // Hotkey ID for Ctrl + L
        private const int HOTKEY_ID = 1;
        private const string validPassword = "1234"; // Valid password for login

        public Form1()
        {
            InitializeComponent();
        }

        // Handle the Login button click
        private void BtnLogin_Click(object sender, EventArgs e)
        {
            string enteredPassword = txtPassword.Text;

            // Check if the entered password matches the valid one
            if (enteredPassword == validPassword)
            {
                MessageBox.Show("Login Successful!");
                LockScreen(); // Lock the screen after successful login
            }
            else
            {
                MessageBox.Show("Incorrect Password!");
            }
        }

        // Method to lock the desktop via hotkey (Ctrl + L)
        private void LockDesktop()
        {
            LockWorkStation(); // Lock the desktop
        }

        // Override WndProc to handle hotkey messages
        protected override void WndProc(ref Message m)
        {
            const int WM_HOTKEY = 0x0312; // Hotkey message identifier

            // Check if the message is a hotkey event
            if (m.Msg == WM_HOTKEY)
            {
                // Check if Ctrl + L hotkey is pressed
                if (m.WParam.ToInt32() == HOTKEY_ID)
                {
                    LockScreen(); // Lock the desktop when Ctrl + L is pressed
                }
            }
            base.WndProc(ref m);
        }

        // Register the hotkey (Ctrl + L) when the form is loaded
        private void Form1_Load(object sender, EventArgs e)
        {
            // Register Ctrl + L as the hotkey (Control + L key)
            RegisterHotKey(this.Handle, HOTKEY_ID, 0x0002 /* Control key */, (uint)Keys.L);
        }

        // Lock the screen by showing the lock form
        private void LockScreen()
        {
            LockForm lockForm = new LockForm(); // Create lock form
            lockForm.ShowDialog(); // Display lock screen form modally
        }

        // Clear the default "Password" text when the user clicks the TextBox
        private void txtPassword_Enter(object sender, EventArgs e)
        {
            if (txtPassword.Text == "Password") // Reset the default text
            {
                txtPassword.Text = "";
            }
        }

        // Set the default "Password" text back when the TextBox loses focus, if it's empty
        private void txtPassword_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPassword.Text)) // Add the default text back
            {
                txtPassword.Text = "Password";
            }
        }
    }
}
