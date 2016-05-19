using System;
using System.Windows.Forms;
using bakkup.StorageHandlers;
using System.Net;
using System.IO;
using System.Diagnostics;
using System.Drawing;

namespace bakkup
{
    public partial class ServicePickerForm : Form
    {

        private GoogleDriveStorageHandler _storageHandler;

        public ServicePickerForm()
        {
            InitializeComponent();           
        }

        private void ServicePickerForm_Load(object sender, EventArgs e)
        {
            //check for version updates
            WebClient client = new WebClient();

            try
            {   //open the text file using a stream reader
                using (Stream stream = client.OpenRead("http://textuploader.com/5bjaq/raw"))
                {
                    StreamReader reader = new StreamReader(stream);
                    string latest = reader.ReadToEnd();

                    if (latest != Program.version && Program.FirstStart == true)
                    {
                        Program.FirstStart = false;

                        DialogResult answer = MessageBox.Show("There is a new update available!\nDownload now?", "Update Found!", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                        if (answer == DialogResult.Yes)
                        {
                            Process.Start("https://github.com/rex706/bakkup");
                            Close();
                        }
                        else if (answer == DialogResult.No)
                        {
                            SelectProviderLabel.ForeColor= Color.Red;
                            SelectProviderLabel.Text = "v" + latest + " update available!";
                        }
                    }
                    else if (latest != Program.version)
                    {
                        SelectProviderLabel.ForeColor = Color.Red;
                        SelectProviderLabel.Text = "v" + latest + " update available!";
                    }
                }
            }
            catch (Exception m)
            {
                //MessageBox.Show(m.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #region GoogleDrive

        private void GoogleDriveButton_MouseEnter(object sender, EventArgs e)
        {
            // ImageList index value for the hover image.
            GoogleDriveButton.ImageIndex = 1;
        }

        private void GoogleDriveButton_MouseLeave(object sender, EventArgs e)
        {
            // ImageList index value for the normal image.
            GoogleDriveButton.ImageIndex = 0;
        }

        private async void GoogleDriveButton_Click(object sender, EventArgs e)
        {
            Program.GD = true;

            _storageHandler = new GoogleDriveStorageHandler();
            if (!await _storageHandler.InitializeHandler())
            {
                //Failed to login.
                MessageBox.Show("Failed to login to Google Drive!", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Console.WriteLine("Login Error Occurred:");
                Console.WriteLine(_storageHandler.LastErrorMessage);
            }
            else
            {
                Console.WriteLine("Login and bakkup folder initialization successful.");
                //buttonUpload.Enabled = buttonTestFolder.Enabled = true;
            }

            Close();
        }

        #endregion
        #region OneDrive

        private void OneDriveButton_MouseEnter(object sender, EventArgs e)
        {
            // ImageList index value for the hover image.
            //OneDriveButton.ImageIndex = 1;
        }

        private void OneDriveButton_MouseLeave(object sender, EventArgs e)
        {
            // ImageList index value for the normal image.
            //OneDriveButton.ImageIndex = 0;
        }

        private void OneDriveButton_Click(object sender, EventArgs e)
        {
            //Program.OD = true;
            //Close();
            MessageBox.Show("Coming Soon");
        }

        #endregion
        #region DropBox

        private void DropBoxButton_MouseEnter(object sender, EventArgs e)
        {
            // ImageList index value for the hover image.
            //DropBoxButton.ImageIndex = 1;
        }

        private void DropBoxButton_MouseLeave(object sender, EventArgs e)
        {
            // ImageList index value for the normal image.
            //DropBoxButton.ImageIndex = 0;
        }

        private void DropBoxButton_Click(object sender, EventArgs e)
        {
            //Program.DB = true;
            //Close();
            MessageBox.Show("Coming Soon");
        }

        #endregion
    }
}
