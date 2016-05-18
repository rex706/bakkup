﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using bakkup.Clients;
using bakkup.StorageHandlers;

namespace bakkup
{
    public partial class ServicePickerForm : Form
    {

        private GoogleDriveStorageHandler _storageHandler;

        public ServicePickerForm()
        {
            InitializeComponent();

            GoogleDriveButton.MouseEnter += new EventHandler(GoogleDriveButton_MouseEnter);
            GoogleDriveButton.MouseLeave += new EventHandler(GoogleDriveButton_MouseLeave);

            DropBoxButton.MouseEnter += new EventHandler(DropBoxButton_MouseEnter);
            DropBoxButton.MouseLeave += new EventHandler(DropBoxButton_MouseLeave);

            OneDriveButton.MouseEnter += new EventHandler(OneDriveButton_MouseEnter);
            OneDriveButton.MouseLeave += new EventHandler(OneDriveButton_MouseLeave);
        }

        private void ServicePickerForm_Load(object sender, EventArgs e)
        {

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
                MessageBox.Show("Failed to login to Google Drive!", "Login Failed", MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
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