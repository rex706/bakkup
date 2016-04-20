using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic;

namespace bakkup
{
    public partial class Form1 : Form
    {
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

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //check for internet
            if (Program.CheckForInternetConnection() == false)
            {
                MessageBox.Show("WARNING: No Internet connection found. \nSave files cannot be fetched but will still attempt to update on exit. Play anyway?", "ERROR", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
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

            gameDirectories = Directory.GetDirectories(SavePath);
            games = Directory.GetDirectories(SavePath);

            //get the folder names instead of the entire path string
            for (int i = 0; i < gameDirectories.Length; i++)
            {
                games[i] = gameDirectories[i].Split(new char[] { Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries).Last();
                games[i] = (i + 1) + ". " + games[i];
            }

            label1.ForeColor = System.Drawing.Color.Blue;
            if (games.Length > 1) label1.Text = games.Length + " games found!";
            else if (games.Length == 0) label1.Text = "No games found!";
            else label1.Text = games.Length + " game found!";

            listBox1.DataSource = games;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectionString = listBox1.GetItemText(listBox1.SelectedItem);
            int dotIdx = selectionString.IndexOf(".");
            selection = Int32.Parse(selectionString.Substring(0, dotIdx)) - 1;
        }

        //select
        private void button1_Click(object sender, EventArgs e)
        {
            int strIdx = selectionString.IndexOf(".") + 2;
            SettingsPath = SavePath + "\\" + games[selection].Substring(strIdx) + "\\bakkup.txt";

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
                label2.ForeColor = System.Drawing.Color.Red;
                label2.Text = "Loading Failed!";
                label2.Visible = true;
                MessageBox.Show(m.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1);
            }

            //copy files to local path
            label2.ForeColor = System.Drawing.Color.Orange;
            label2.Text = "Copying Files...";
            label2.Visible = true;
            Program.DirectoryCopy(SavePath + "\\" + games[selection].Substring(strIdx), localSavePath, true);
            label2.ForeColor = System.Drawing.Color.Green;
            label2.Text = "Local Transfer Complete!";

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
            label2.ForeColor = System.Drawing.Color.Orange;
            label2.Text = "Copying Files...";
            Program.DirectoryCopy(localSavePath, SavePath + "\\" + games[selection].Substring(strIdx), true);
            label2.ForeColor = System.Drawing.Color.Green;
            label2.Text = "Cloud Backup Complete!";
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
            label2.ForeColor = System.Drawing.Color.Orange;
            label2.Text = "Copying Files...";
            label2.Visible = true;
            Program.DirectoryCopy(localSavePath, newDir, true);
            label2.ForeColor = System.Drawing.Color.Green;
            label2.Text = "Cloud Backup Complete!";

            refreshList();
        }

        //refresh
        private void button3_Click(object sender, EventArgs e)
        {
            refreshList();
        }

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
            label1.ForeColor = System.Drawing.Color.Blue;
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
    }
}