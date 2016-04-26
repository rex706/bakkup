using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Microsoft.VisualBasic;

namespace bakkup
{
    public partial class Form1 : Form
    {
        public string argument = null;
        public bool argFlag = false;
        public bool compareFlag = false;

        public string[] gameDirectories;
        public string[] games;

        public int selection;
        public string selectionString;
        public string SavePath;
        public string SettingsPath;

        public string gameName;
        public string localSavePath;
        public string exePath;
        public string parameters;

        public Form1(string[] args)
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

        private void Form1_Load(object sender, EventArgs e)
        {
            //check for internet
            if (Program.CheckForInternetConnection() == false)
            {
                MessageBox.Show("No Internet connection found! \nSave files cannot be fetched but will still attempt to update on exit. \nThis will overwrite the previous cloud save once connection is established. Play anyway?", "WARNING", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            }

            string GooglePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Google Drive";
            SavePath = GooglePath + "\\bakkups";

            //make sure google drive directory exists
            if (!Directory.Exists(GooglePath))
            {
                MessageBox.Show("Could not locate Google Drive directory.\n Make sure it is installed and signed in, then try again.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1);
            }

            //make sure save path exists
            if (!Directory.Exists(SavePath))
            {
                MessageBox.Show("Could not locate bakkups directory.\nIt will now be created on your Google Drive.");
                Directory.CreateDirectory(SavePath);
            }

            refreshList();

            //if user input arguments
            if(argFlag == true)
            {
                //make sure argument exists
                if (Program.compareString(games, argument) == true)
                {
                    compareFlag = true;
                    button1.PerformClick();
                    MessageBox.Show("Backup Complete!");
                    Environment.Exit(1);
                }
                else
                {
                    MessageBox.Show("Argument error!\nCould not find directory for desired game:\n\n" + argument.Substring(1), "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Environment.Exit(1);
                }
            }
        }

        //update current selected item
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectionString = listBox1.GetItemText(listBox1.SelectedItem);
            int dotIdx = selectionString.IndexOf(".");
            selection = Int32.Parse(selectionString.Substring(0, dotIdx)) - 1;
        }

        //select
        private void button1_Click(object sender, EventArgs e)
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
                gameName = games[selection].Substring(strIdx);
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
                Environment.Exit(1);
            }

            //copy files to local path
            label1.ForeColor = Color.Orange;
            label1.Text = "Copying Files...";
            Program.DirectoryCopy(SavePath + "\\" + gameName, localSavePath, true);
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
                MessageBox.Show (m.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1);
            }

            //copy files to google drive
            label1.ForeColor = Color.Orange;
            label1.Text = "Copying Files...";
            Program.DirectoryCopy(localSavePath, SavePath + "\\" + gameName, true);
            label1.ForeColor = Color.Green;
            label1.Text = "Cloud Backup Complete!";
        }

        //new
        private void button2_Click(object sender, EventArgs e)
        {
            //prompt user to locate local save folder
            FolderBrowserDialog fBrowser = new FolderBrowserDialog();
            fBrowser.Description = "Select local save folder";

            if (fBrowser.ShowDialog() == DialogResult.OK) localSavePath = fBrowser.SelectedPath;

            //prompt user to locate game exe
            OpenFileDialog exeBrowser = new OpenFileDialog();
            exeBrowser.Filter = "Game Executable (*.exe)|*.exe";
            exeBrowser.FilterIndex = 1;
            exeBrowser.Multiselect = false;

            if (exeBrowser.ShowDialog() == DialogResult.OK) exePath = exeBrowser.FileName;

            //get exe name from path string
            int last_slash_idx = exePath.LastIndexOf('\\');
            string exeName = exePath.Substring(last_slash_idx+1);
            int dotIdx = exeName.IndexOf('.');
            exeName = exeName.Substring(0, dotIdx);

            //prompt user to name new game folder (usually the name of the game)
            gameName = Interaction.InputBox("Enter name of Game:", "bakkup", exeName, Width * 2, Height * 2);

            //prompt for any perameters
            parameters = Interaction.InputBox("OPTIONAL - Enter any parameters:", "bakkup", "", Width * 2, Height * 2);
            
            //create new directory
            string newDir = SavePath + "\\" + gameName;
            Directory.CreateDirectory(newDir);

            //check for invalid input/cancels
            if(newDir == null || exePath == null || gameName == null)
            {
                MessageBox.Show("One or more inputs were invalid or cancelled. \nPlease try again.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1);
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
            Program.DirectoryCopy(localSavePath, newDir, true);
            label1.ForeColor = Color.Green;
            label1.Text = "Cloud Backup Complete!";

            refreshList();
        }

        //refresh
        private void button3_Click(object sender, EventArgs e)
        {
            refreshList();
        }

        //refresh and populate listbox
        private void refreshList()
        {
            gameDirectories = Directory.GetDirectories(SavePath);
            games = Directory.GetDirectories(SavePath);

            //get the folder names instead of the entire path string
            for (int i = 0; i < gameDirectories.Length; i++)
            {
                games[i] = gameDirectories[i].Split(new char[] { Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries).Last();
                games[i] = (i + 1) + ". " + games[i];
            }

            //display number of games found
            label1.ForeColor = Color.Blue;
            if (games.Length > 1)
            {
                label1.Text = games.Length + " games found!";
                button1.Enabled = true;
            }
            else if (games.Length == 0)
            {
                label1.Text = "No games found!";
                button1.Enabled = false;
            }
            else
            {
                button1.Enabled = true;
                label1.Text = games.Length + " game found!";
            }

            //populate listbox
            listBox1.DataSource = games;
        }

        //auto load checkbox
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                //hide backup buttons and show select button
                button4.Hide();
                button5.Hide();
                button1.Show();
            }
            else if (!checkBox1.Checked)
            {
                //hide select button and show backup buttons
                button1.Hide();
                button4.Show();
                button5.Show();
            }
        }

        //retrieve
        private void button4_Click(object sender, EventArgs e)
        {
            int strIdx = selectionString.IndexOf(".") + 2;
            gameName = games[selection].Substring(strIdx);
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
                Environment.Exit(1);
            }

            //copy files to local path
            label1.ForeColor = Color.Orange;
            label1.Text = "Copying Files...";
            Program.DirectoryCopy(SavePath + "\\" + gameName, localSavePath, true);
            label1.ForeColor = Color.Green;
            label1.Text = "Local Transfer Complete!";
        }

        //backup
        private void button5_Click(object sender, EventArgs e)
        {
            int strIdx = selectionString.IndexOf(".") + 2;
            gameName = games[selection].Substring(strIdx);
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
                Environment.Exit(1);
            }

            //copy files to google drive
            label1.ForeColor = Color.Orange;
            label1.Text = "Copying Files...";
            Program.DirectoryCopy(localSavePath, SavePath + "\\" + gameName, true);
            label1.ForeColor = Color.Green;
            label1.Text = "Cloud Backup Complete!";
        }
    }
}