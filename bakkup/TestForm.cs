using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using bakkup.Clients;
using bakkup.StorageHandlers;

namespace bakkup
{
    public partial class TestForm : Form
    {
        private GoogleDriveStorageHandler _storageHandler;

        public TestForm()
        {
            InitializeComponent();
        }

        private async void buttonStartLogin_Click(object sender, EventArgs e)
        {
            _storageHandler = new GoogleDriveStorageHandler();
            if (!await _storageHandler.InitializeHandler())
            {
                //Failed to login.
                MessageBox.Show("Failed to login to Google Drive!", "Login Failed", MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
                Console.WriteLine("Login Error Occurred:");
                Console.WriteLine(_storageHandler.LastErrorMessage);
            }
            else
            {
                Console.WriteLine("Login and bakkup folder initialization successful.");
                buttonUpload.Enabled = buttonBakkupFolder.Enabled = true;
            }
                
        }

        private async void buttonUpload_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = false;
            dialog.Filter = "All Files (*.*)|*.*";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                //Open a filestream to the file.
                using (var stream = new FileStream(dialog.FileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    await _storageHandler.UploadSmallTestFile(dialog.SafeFileName,
                        stream, new Progress<int>(ReportUploadProgressCallback));
                    if (_storageHandler.LastError != OAuthClientResult.Success)
                    {
                        Console.WriteLine("Error uploading file: " + _storageHandler.LastErrorMessage);
                    }
                    else
                    {
                        Console.WriteLine("File uploaded successfully!");
                    }
                }
            }
        }

        private void ReportUploadProgressCallback(int progress)
        {
            MethodInvoker invokerDelegate = delegate()
            {
                progressBarProgress.Value = progress;
            };
            progressBarProgress.Invoke(invokerDelegate);
        }

        private async void buttonBakkupFolder_Click(object sender, EventArgs e)
        {
            await _storageHandler.CreateRemoteTestFolder();
            if(_storageHandler.LastError == OAuthClientResult.Success)
                Console.WriteLine("The test folder has been created successfully!");
            else
                Console.WriteLine("Failed to create the test folder: " + _storageHandler.LastErrorMessage);
        }
    }
}
