using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using System.Collections.Generic;
using System.Threading;

namespace bakkup
{
    public partial class MainForm : Form
    {
        //Nkosi Note: None of these variables should be declared public.

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

        //Nkosi Note: UI elements should always have a default constructor that takes no parameters.
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

        private void MainForm_Load(object sender, EventArgs e)
        {
            //check for internet
            if (Util.CheckForInternetConnection() == false)
            {
                DialogResult answer = MessageBox.Show("No Internet connection found! \nSave files cannot be fetched but will still attempt to update on game exit if Auto-Run is enabled. \nThis will overwrite the previous cloud save once connection is established. Play anyway?", "WARNING", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (answer == DialogResult.No)
                    Close(); //Nkosi Note: Closing the window passed to Application.Run will exit the program.
            }

            string GooglePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Google Drive";
            SavePath = GooglePath + "\\bakkups";

            //make sure google drive directory exists
            if (!Directory.Exists(GooglePath))
            {
                MessageBox.Show("Could not locate Google Drive directory.\n Make sure it is installed and signed in, then try again.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //Environment.Exit(0);
                //Nkosi Note: Closing the window passed to Application.Run will exit the program.
                Close();
            }

            //make sure save path exists
            if (!Directory.Exists(SavePath))
            {
                MessageBox.Show("Could not locate bakkups directory.\nIt will now be created on your Google Drive.", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Directory.CreateDirectory(SavePath);
            }

            refreshList();

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
            catch(Exception m)
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

            refreshList();
        }

        //refresh
        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            refreshList();
        }

        //refresh and populate listbox
        private void refreshList()
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

                    WriteTimes[i] = Directory.GetLastWriteTime(gameDirectories[i]).ToString();
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

                refreshList();
            }
        }
    }
}