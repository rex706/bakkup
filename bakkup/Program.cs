using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Forms;

namespace bakkup
{
    static class Program
    {
        /// <summary>
        /// Copies save files to and from a Google Drive folder to keep saves up to date and accessable from anywhere.
        /// </summary>

        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1(args));
        }

        public static bool CheckForInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                {
                    using (var stream = client.OpenRead("http://www.google.com"))
                    {
                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            //get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException("Source directory does not exist or could not be found: "+ sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();

            //if the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            //get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, true);
            }

            //if copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }

        public static bool compareString(string[] dirInput, string userInput)
        {
            for(int i = 0; i < dirInput.Length; i++)
            {
                int dotIdx = dirInput[i].IndexOf(".") + 2;
                bool ret = dirInput[i].Substring(dotIdx).SequenceEqual(userInput.Substring(1));

                if (ret == true) return true;
            }

            return false;
        }
    }
}