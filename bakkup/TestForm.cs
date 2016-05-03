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
            GoogleDriveClient client = new GoogleDriveClient(this);
            await client.PerformLogin();
            Console.WriteLine("Done");
        }
    }
}
