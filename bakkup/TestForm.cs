using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using bakkup.Clients;

namespace bakkup
{
    public partial class TestForm : Form
    {
        public TestForm()
        {
            InitializeComponent();
        }

        private async void buttonStartLogin_Click(object sender, EventArgs e)
        {
            GoogleDriveClient client = new GoogleDriveClient();
            if (!await client.Login())
            {
                //Failed to login.
                MessageBox.Show("Failed to login to Google Drive!", "Login Failed", MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
                Console.WriteLine("Login Error Occurred:");
                Console.WriteLine(client.LastErrorMessage);
            }
            else
                Console.WriteLine("Login successful.");
        }
    }
}
