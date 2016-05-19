//TODO: Add a place within MainForm to check for updates.

using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using bakkup.StorageHandlers;

namespace bakkup
{
    public partial class MainForm : Form
    {
        #region Variables

        private string argument = null;
        private bool argFlag = false;
        private bool compareFlag = false;

        private string[] gameDirectories;
        private List<string> gameList;
        private List<string> WriteTimesList;

        private int selection;
        private string selectionString;
        private string SavePath;
        private string SettingsPath;

        private string gameName;
        private string localSavePath;
        private string exePath;
        private string parameters;

        private IStorageHandler _storageHandler;

        #endregion

        #region Constructors

        public MainForm()
        {
            InitializeComponent();
        }

        public MainForm(string[] args)
        {
            //user specified specific game via shortcut argument
            if (args.Length > 0)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    if (i == args.Length - 1) argument += args[i];
                    else argument += args[i] + " ";
                }
                argFlag = true;
            }

            InitializeComponent();
        }

        #endregion

        #region Window Event Listeners

        private async void MainForm_Load(object sender, EventArgs e)
        {
            //Check for internet
            if (Util.CheckForInternetConnection() == false)
            {
                DialogResult answer = MessageBox.Show("No Internet connection found! \nSave files cannot be fetched but will still attempt to update on game exit if Auto-Run is enabled. \nThis will overwrite the previous cloud save once connection is established. Play anyway?", "WARNING", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (answer == DialogResult.No)
                    Close();
            }

            //Check for a new version.
            int updateResult = await CheckForUpdate();
            if (updateResult == -1)
            {
                //Some error occurred.
                //TODO: Handle this error.
            }
            else if (updateResult == 1)
            {
                //An update is available, but user has chosen not to update.
                //TODO: Update the UI to show that an update is available.
            }
            else if (updateResult == 2)
            {
                //An update is available, and the user has chosen to update.
                //TODO: Exit the application. Later, initiate a process that downloads
                //new updated binaries.
                Close();
            }

            //Show the provider dialog.
            var form = new ServicePickerForm();
            form.ShowDialog();
            if (form.SelectedStorageHandler == null)
            {
                //Provider window was closed and no storage handler was successfully set, so just exit.
                Close();
                return;
            }

            //Set the new storage handler. It was also initialized 
            _storageHandler = form.SelectedStorageHandler;

            //Show the image of the currently selected storage handler.
            switch (_storageHandler.ProviderType)
            {
                case StorageProviders.GoogleDrive:
                    serviceLabel.ImageIndex = 0;
                    break;
                case StorageProviders.OneDrive:
                    serviceLabel.ImageIndex = 1;
                    break;
                case StorageProviders.DropBox:
                    serviceLabel.ImageIndex = 2;
                    break;
            }

            linkLabelVersion.Text = "v" + Assembly.GetExecutingAssembly().GetName().Version;

            string GooglePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Google Drive";
            SavePath = GooglePath + "\\bakkups";

            //make sure google drive directory exists
            if (!Directory.Exists(GooglePath))
            {
                MessageBox.Show("Could not locate Google Drive directory.\n Make sure it is installed and signed in, then try again.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
            }

            //make sure save path exists
            if (!Directory.Exists(SavePath))
            {
                MessageBox.Show("Could not locate bakkups directory.\nIt will now be created on your Google Drive.", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Directory.CreateDirectory(SavePath);
            }

            RefreshList();

            //if user input arguments
            if (argFlag == true)
            {
                //make sure argument exists
                if (Util.CompareString(gameList, argument) == true)
                {
                    compareFlag = true;
                    buttonSelect.PerformClick();
                    MessageBox.Show("Backup Complete!");
                    Close();
                }
                else
                {
                    MessageBox.Show("Argument error!\nCould not find directory for desired game:\n\n" + argument.Substring(1), "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Close();
                }
            }
        }

        //update current selected item
        private void listBoxBakkups_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Issue with ListBox's in Windows Forms. There can be calls here when the number
            //of selected items is 0. When this happens, do nothing.
            if (listBoxWriteTime.SelectedItems.Count == 0)
                return;

            listBoxWriteTime.SelectedIndex = listBoxBakkups.SelectedIndex;

            selectionString = listBoxBakkups.GetItemText(listBoxBakkups.SelectedItem);
            int dotIdx = selectionString.IndexOf(".");
            selection = Int32.Parse(selectionString.Substring(0, dotIdx)) - 1;
        }

        private void listBoxWriteTime_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Issue with ListBox's in Windows Forms. There can be calls here when the number
            //of selected items is 0. When this happens, do nothing.
            if (listBoxBakkups.SelectedItems.Count == 0)
                return;

            listBoxBakkups.SelectedIndex = listBoxWriteTime.SelectedIndex;

            selectionString = listBoxBakkups.GetItemText(listBoxBakkups.SelectedItem);
            int dotIdx = selectionString.IndexOf(".");
            selection = Int32.Parse(selectionString.Substring(0, dotIdx)) - 1;
        }

        //select
        private void buttonSelect_Click(object sender, EventArgs e)
        {
            //check if user input argument
            if (argFlag == true && compareFlag == true)
            {
                gameName = argument.Substring(1);
                SettingsPath = SavePath + "\\" + gameName + "\\bakkup.txt";
            }
            else
            {
                int strIdx = selectionString.IndexOf(".") + 2;
                gameName = gameList[selection].Substring(strIdx);
                SettingsPath = SavePath + "\\" + gameName + "\\bakkup.txt";
            }

            try
            {   //open the text file using a stream reader
                using (StreamReader sr = new StreamReader(SettingsPath))
                {
                    localSavePath = sr.ReadLine();
                    exePath = sr.ReadLine();
                    parameters = sr.ReadLine();
                }
            }
            catch (Exception m)
            {
                label1.ForeColor = Color.Red;
                label1.Text = "Local Transfer Failed!";
                MessageBox.Show(m.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            //copy files to local path
            label1.ForeColor = Color.Orange;
            label1.Text = "Copying Files...";
            Util.DirectoryCopy(SavePath + "\\" + gameName, localSavePath, true);
            label1.ForeColor = Color.Green;
            label1.Text = "Local Transfer Complete!";

            //get working directory from exePath variable
            int last_slash_idx = exePath.LastIndexOf('\\');
            string workingDirectory = exePath.Substring(0, last_slash_idx);

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = true;
            startInfo.FileName = exePath;
            startInfo.WorkingDirectory = workingDirectory;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.Arguments = parameters;

            try
            {
                //sart the process with the info specified and wait for close
                using (Process exeProcess = Process.Start(startInfo))
                {
                    exeProcess.WaitForExit();
                }
            }
            catch (Exception m)
            {
                MessageBox.Show(m.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            //copy files to google drive
            label1.ForeColor = Color.Orange;
            label1.Text = "Copying Files...";
            Util.DirectoryCopy(localSavePath, SavePath + "\\" + gameName, true);
            label1.ForeColor = Color.Green;
            label1.Text = "Cloud Backup Complete!";
        }

        //new
        private void buttonNewBackup_Click(object sender, EventArgs e)
        {
            //prompt user to locate local save folder
            FolderBrowserDialog fBrowser = new FolderBrowserDialog();
            fBrowser.Description = "Select local save folder";

            if (fBrowser.ShowDialog() == DialogResult.OK) localSavePath = fBrowser.SelectedPath;
            else
            {
                MessageBox.Show("You must select a valid local save directory!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            //prompt user to locate game exe
            OpenFileDialog exeBrowser = new OpenFileDialog();
            exeBrowser.Filter = "Game Executable (*.exe)|*.exe";
            exeBrowser.FilterIndex = 1;
            exeBrowser.Multiselect = false;

            if (exeBrowser.ShowDialog() == DialogResult.OK) exePath = exeBrowser.FileName;
            else
            {
                MessageBox.Show("You must select a valid game exe!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            //get exe name from path string
            int last_slash_idx = exePath.LastIndexOf('\\');
            string exeName = exePath.Substring(last_slash_idx + 1);
            int dotIdx = exeName.IndexOf('.');
            exeName = exeName.Substring(0, dotIdx);

            //prompt user to name new game folder (usually the name of the game)
            gameName = Interaction.InputBox("Enter name of Game:", "bakkup", exeName, Width * 2, Height * 2);
            while (gameName.Length < 3)
            {
                MessageBox.Show("Name must be more than 2 characters long!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                gameName = Interaction.InputBox("Enter name of Game:", "bakkup", exeName, Width * 2, Height * 2);
            }

            //prompt for any perameters
            parameters = Interaction.InputBox("OPTIONAL - Enter any parameters:", "bakkup", "", Width * 2, Height * 2);

            //create new directory
            string newDir = SavePath + "\\" + gameName;
            try
            {
                Directory.CreateDirectory(newDir);
            }
            catch (Exception m)
            {
                MessageBox.Show(m.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            //create settings file
            using (StreamWriter sw = File.CreateText(newDir + "\\bakkup.txt"))
            {
                sw.WriteLine(localSavePath);
                sw.WriteLine(exePath);
                sw.WriteLine(parameters);
            }

            //copy files to google drive
            label1.ForeColor = Color.Orange;
            label1.Text = "Copying Files...";
            Util.DirectoryCopy(localSavePath, newDir, true);
            label1.ForeColor = Color.Green;
            label1.Text = "Cloud Backup Complete!";

            RefreshList();
        }

        //refresh
        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            RefreshList();
        }

        //auto load checkbox
        private void checkBoxAutoRun_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxAutoRun.Checked)
            {
                //hide backup buttons and show select button
                buttonRetrieve.Hide();
                buttonBackup.Hide();
                buttonSelect.Show();
            }
            else if (!checkBoxAutoRun.Checked)
            {
                //hide select button and show backup buttons
                buttonSelect.Hide();
                buttonRetrieve.Show();
                buttonBackup.Show();
            }
        }

        //retrieve
        private void buttonRetrieve_Click(object sender, EventArgs e)
        {
            int strIdx = selectionString.IndexOf(".") + 2;
            gameName = gameList[selection].Substring(strIdx);
            SettingsPath = SavePath + "\\" + gameName + "\\bakkup.txt";

            try
            {   //open the text file using a stream reader
                using (StreamReader sr = new StreamReader(SettingsPath))
                {
                    localSavePath = sr.ReadLine();
                    exePath = sr.ReadLine();
                    parameters = sr.ReadLine();
                }
            }
            catch (Exception m)
            {
                label1.ForeColor = Color.Red;
                label1.Text = "Local Transfer Failed!";
                MessageBox.Show(m.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            //copy files to local path
            label1.ForeColor = Color.Orange;
            label1.Text = "Copying Files...";
            Util.DirectoryCopy(SavePath + "\\" + gameName, localSavePath, true);
            label1.ForeColor = Color.Green;
            label1.Text = "Local Transfer Complete!";
        }

        //backup
        private void buttonBackup_Click(object sender, EventArgs e)
        {
            int strIdx = selectionString.IndexOf(".") + 2;
            gameName = gameList[selection].Substring(strIdx);
            SettingsPath = SavePath + "\\" + gameName + "\\bakkup.txt";

            try
            {   //open the text file using a stream reader
                using (StreamReader sr = new StreamReader(SettingsPath))
                {
                    localSavePath = sr.ReadLine();
                    exePath = sr.ReadLine();
                    parameters = sr.ReadLine();
                }
            }
            catch (Exception m)
            {
                label1.ForeColor = Color.Red;
                label1.Text = "Cloud Backup Failed!";
                MessageBox.Show(m.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            //copy files to google drive
            label1.ForeColor = Color.Orange;
            label1.Text = "Copying Files...";
            Util.DirectoryCopy(localSavePath, SavePath + "\\" + gameName, true);
            label1.ForeColor = Color.Green;
            label1.Text = "Cloud Backup Complete!";
        }

        //version number click
        private void linkLabelVersion_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //open browser to github page
            Process.Start("https://github.com/rex706/bakkup");
        }

        //remove
        private void buttonRemove_Click(object sender, EventArgs e)
        {
            DialogResult answer = MessageBox.Show("Are you sure you want to permanently delete this entry?", "WARNING", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            //if yes, delete
            if (answer == DialogResult.Yes)
            {
                int strIdx = selectionString.IndexOf(".") + 2;
                gameName = gameList[selection].Substring(strIdx);

                Directory.Delete(SavePath + "\\" + gameName, true);

                RefreshList();
            }
            else
                return;
        }

        //button up/down events to simulate click down animation
        private void buttonRefresh_MouseDown(object sender, MouseEventArgs e)
        {
            buttonRefresh.ImageIndex = 3;
        }

        private void buttonRefresh_MouseUp(object sender, MouseEventArgs e)
        {
            buttonRefresh.ImageIndex = 0;
        }

        private void buttonNewBackup_MouseDown(object sender, MouseEventArgs e)
        {
            buttonNewBackup.ImageIndex = 4;
        }

        private void buttonNewBackup_Mouseup(object sender, MouseEventArgs e)
        {
            buttonNewBackup.ImageIndex = 1;
        }

        private void buttonRemove_MouseDown(object sender, MouseEventArgs e)
        {
            buttonRemove.ImageIndex = 5;
        }

        private void buttonRemove_Mouseup(object sender, MouseEventArgs e)
        {
            buttonRemove.ImageIndex = 2;
        }

        private async void serviceLabel_Click(object sender, EventArgs e)
        {
            //Show the service provider selector window.
            var window = new ServicePickerForm();
            window.LoadedProvider = _storageHandler.ProviderType;
            window.ShowDialog();
            //Do nothing if either no service provider was selected, or the service provider
            //is the same as the currently loaded provider. This way, the current storage
            //handler remains loaded and its state is maintained.
            if (window.SelectedStorageHandler == null ||
                window.SelectedStorageHandler.ProviderType == _storageHandler.ProviderType)
                return;

            //Cleanup the current storage handler.

            //Initialize the newly selected storage handler.


            //Reload the Mainform with the new storage handler data.

        }

        #endregion

        #region Private Methods

        //Checks if an update is available. 
        //-1 for check error, 0 for no update, 1 for update is available, 2 for perform update.
        private static async Task<int> CheckForUpdate()
        {
            //Nkosi Note: Always use asynchronous versions of network and IO methods.

            //Check for version updates
            var client = new HttpClient();
            client.Timeout = new TimeSpan(0, 0, 0, 10);
            try
            {
                //open the text file using a stream reader
                using (Stream stream = await client.GetStreamAsync("http://textuploader.com/5bjaq/raw"))
                {
                    StreamReader reader = new StreamReader(stream);
                    Version latest = Version.Parse(reader.ReadToEnd());
                    Version current = Assembly.GetExecutingAssembly().GetName().Version;

                    if (latest.Major != current.Major || latest.Minor != current.Minor)
                    {
                        DialogResult answer = MessageBox.Show("There is a new update available!\nDownload now?", "Update Found!", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                        if (answer == DialogResult.Yes)
                        {
                            //TODO: Later on, remove this and replace with automated process of downloading new binaries.
                            Process.Start("https://github.com/rex706/bakkup");
                            //Update is available, and user wants to update. Requires app to close.
                            return 2;
                        }
                        //Update is available, but user chose not to update just yet.
                        return 1;
                    }
                }

                //No update available.
                return 0;
            }
            catch (Exception m)
            {
                MessageBox.Show("Failed to check for update.\n" + m.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }

        //refresh and populate listbox
        private void RefreshList()
        {
            gameDirectories = Directory.GetDirectories(SavePath);
            string[] games = new string[gameDirectories.Length];
            string[] WriteTimes = new string[gameDirectories.Length];
            int valid = 0;

            //get the folder names instead of the entire path string and check if a bakkup.txt exists to be valid
            for (int i = 0; i < gameDirectories.Length; i++)
            {
                if (File.Exists(gameDirectories[i] + "\\bakkup.txt"))
                {
                    valid++;
                    games[i] = gameDirectories[i].Split(new char[] { Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries).Last();
                    games[i] = (valid) + ". " + games[i];

                    //look through all the files in the current path and get the most recent last write date
                    string[] fileEntries = Directory.GetFiles(gameDirectories[i]);
                    for (int j = 0; j < fileEntries.Length; j++)
                    {
                        DateTime ftime = File.GetLastWriteTime(fileEntries[0]);
                        DateTime ftime2 = File.GetLastWriteTime(fileEntries[j]);

                        if (ftime < ftime2)
                            fileEntries[0] = fileEntries[j];
                    }

                    WriteTimes[i] = Directory.GetLastWriteTime(fileEntries[0]).ToString();
                }
                else
                {
                    games[i] = "@@@";
                    WriteTimes[i] = "@@@";
                }
            }

            gameList = new List<string>(games);
            gameList.RemoveAll(item => item == "@@@");

            WriteTimesList = new List<string>(WriteTimes);
            WriteTimesList.RemoveAll(item => item == "@@@");

            //display number of games found
            label1.ForeColor = Color.Blue;
            if (gameList.Count > 1)
            {
                label1.Text = gameList.Count + " bakkups found!";
                buttonSelect.Enabled = true;
                checkBoxAutoRun.Enabled = true;
            }
            else if (gameList.Count == 0)
            {
                label1.Text = "No bakkups found!";
                buttonSelect.Enabled = false;
                checkBoxAutoRun.Enabled = false;
            }
            else
            {
                label1.Text = gameList.Count + " bakkup found!";
                buttonSelect.Enabled = true;
                checkBoxAutoRun.Enabled = true;
            }

            //populate listbox
            listBoxBakkups.DataSource = gameList;
            listBoxWriteTime.DataSource = WriteTimesList;
        }

        #endregion
    }
}