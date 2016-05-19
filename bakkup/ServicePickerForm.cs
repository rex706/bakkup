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

namespace bakkup
{
    public partial class ServicePickerForm : Form
    {
        //private GoogleDriveStorageHandler _storageHandler;

        public ServicePickerForm()
        {
            InitializeComponent();           
        }

        private async Task ServicePickerForm_Load(object sender, EventArgs e)
        {
            //Nkosi Note: Always use asynchronous versions of network and IO methods.
            //check for version updates
            var client = new HttpClient();

            try
            {    
                //open the text file using a stream reader
                using (Stream stream = await client.GetStreamAsync("http://textuploader.com/5bjaq/raw"))
                {
                    StreamReader reader = new StreamReader(stream);
                    Version latest = Version.Parse(reader.ReadToEnd());
                    Version current = Assembly.GetExecutingAssembly().GetName().Version;
                    
                    if (!latest.Equals(current) && Program.FirstStart == true)
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
                    else if (!latest.Equals(current))
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
                        var dialogResult = MessageBox.Show(description, "Login Error", MessageBoxButtons.RetryCancel,
                            MessageBoxIcon.Error);
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
            //Program.GD = true;

            await LoadStorageHandler(new GoogleDriveStorageHandler());

            //_storageHandler = new GoogleDriveStorageHandler();
            //if (!await _storageHandler.InitializeHandler())
            //{
            //    //Failed to login.
            //    MessageBox.Show("Failed to login to Google Drive!", "Login Failed", MessageBoxButtons.OK,
            //        MessageBoxIcon.Exclamation);
            //    Console.WriteLine("Login Error Occurred:");
            //    Console.WriteLine(_storageHandler.LastErrorMessage);
            //}
            //else
            //{

            //    //buttonUpload.Enabled = buttonTestFolder.Enabled = true;
            //}

            //Close();
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
            //Nkosi Note: Later, use the same logic as Google Drive.
            //await LoadStorageHandler(new OneDriveStorageHandler());

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
            //Nkosi Note: Later, use the same logic as Google Drive.
            //await LoadStorageHandler(new DropBoxStorageHandler());

            //Program.DB = true;
            //Close();
            MessageBox.Show("Coming Soon");
        }

        #endregion
    }
}
