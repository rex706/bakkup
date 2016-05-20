using System;
using System.Windows.Forms;
using bakkup.StorageHandlers;
using System.Net;
using System.IO;
using System.Diagnostics;
using System.Drawing;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using bakkup.Clients;

namespace bakkup
{
    public partial class ServicePickerForm : Form
    {
        public ServicePickerForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets the initialized storage handler that was chosen by the user. Null if no storage handler
        /// was selected.
        /// </summary>
        public IStorageHandler SelectedStorageHandler { get; private set; }

        /// <summary>
        /// Sets the storage provider that is loaded in the application. This will disable the
        /// button for the provider, since it is already loaded.
        /// </summary>
        public StorageProviders LoadedProvider
        {
            set
            {
                if (value == StorageProviders.GoogleDrive)
                    GoogleDriveButton.Enabled = false;
                else if (value == StorageProviders.OneDrive)
                    OneDriveButton.Enabled = false;
                else if (value == StorageProviders.DropBox)
                    DropBoxButton.Enabled = false;
            }
        }

        private void ServicePickerForm_Load(object sender, EventArgs e)
        {
            
        }

        //Nkosi Note: Use this method to take advantage of the fact that IStorageHandler is an interface. 
        //This eliminates the need to repeat code between all service providers.
        /// <summary>
        /// Attempts to load a storage handler.
        /// </summary>
        /// <param name="handler">An IStorageHandler to initialize.</param>
        /// <returns>A value indicating success or failure of the operation.</returns>
        private async Task LoadStorageHandler(IStorageHandler handler)
        {
            while (true)
            {
                if (!await handler.InitializeHandler())
                {
                    //Unable to initialize the storage handler. Show an error message box for any error
                    //other than the user cancelled authorization.
                    if (handler.LastError != OAuthClientResult.UserCancelled)
                    {
                        //Something went wrong. Ask if the user would like to try again.
                        //TODO: Look at the error that occurred and inform user what to do instead of just try again.
                        var description =
                            string.Format("An error occured while logging into {0}.\n{1}\n{2}\nTry again?",
                                handler.ProviderName, handler.LastError.ToString(), handler.LastErrorMessage);
                        var dialogResult = MessageBox.Show(description, "Login Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
                        if (dialogResult == DialogResult.Cancel)
                            return;
                    }
                    else
                    {
                        //The user cancelled the login process, so do not ask if they want to try again.
                        //Leave the ServicePickerWindow open (by not calling Close) so the user can choose
                        //another service or simply exit.
                        return;
                    }
                }
                else
                {
                    //Login was successful.
                    Console.WriteLine("Login and bakkup folder initialization successful.");
                    SelectedStorageHandler = handler;
                    Close();
                    return; 
                }
                    
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
            //Nkosi Note: Use a property of ServicePickerForm instead. Take advantage of the fact
            //that GoogleDriveStorageHandler implements IStorageHandler and eliminate redundant code
            //(OneDrive and DropBox click event code would be the same) and the singletons (GD, DB, OD)
            //in one go.

            await LoadStorageHandler(new GoogleDriveStorageHandler());
        }

        #endregion

        #region OneDrive

        private void OneDriveButton_MouseEnter(object sender, EventArgs e)
        {
            // ImageList index value for the hover image.
            OneDriveButton.ImageIndex = 3;
        }

        private void OneDriveButton_MouseLeave(object sender, EventArgs e)
        {
            // ImageList index value for the normal image.
            OneDriveButton.ImageIndex = 2;
        }

        private void OneDriveButton_Click(object sender, EventArgs e)
        {
            //Nkosi Note: Later, use the same logic as Google Drive.
            //await LoadStorageHandler(new OneDriveStorageHandler());

            MessageBox.Show("Coming Soon");
        }

        #endregion

        #region DropBox

        private void DropBoxButton_MouseEnter(object sender, EventArgs e)
        {
            // ImageList index value for the hover image.
            DropBoxButton.ImageIndex = 5;
        }

        private void DropBoxButton_MouseLeave(object sender, EventArgs e)
        {
            // ImageList index value for the normal image.
            DropBoxButton.ImageIndex = 4;
        }

        private void DropBoxButton_Click(object sender, EventArgs e)
        {
            //Nkosi Note: Later, use the same logic as Google Drive.
            //await LoadStorageHandler(new DropBoxStorageHandler());

            MessageBox.Show("Coming Soon");
        }

        #endregion
    }
}
