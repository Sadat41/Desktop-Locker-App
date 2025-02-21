using System;
using System.Windows.Forms;

namespace DesktopLockApp
{
    public partial class PasswordPromptForm : Form
    {
        public string Password { get; private set; }

        public PasswordPromptForm()
        {
            InitializeComponent();
        }

        private void PasswordPromptForm_Load(object sender, EventArgs e)
        {
            // Automatically focus on the password TextBox
            txtPassword.Focus();
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            Password = txtPassword.Text;
            this.DialogResult = DialogResult.OK; // Close the form with OK result
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel; // Close the form with Cancel result
            this.Close();
        }

        // Handle the TextChanged event for txtPassword (if needed)
        private void txtPassword_TextChanged(object sender, EventArgs e)
        {
            // Your logic here, if you want to do something when the text changes
        }
    }
}
