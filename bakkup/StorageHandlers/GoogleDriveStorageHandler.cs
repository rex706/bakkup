using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bakkup.Clients;

namespace bakkup.StorageHandlers
{
    /// <summary>
    /// Represents a Google Drive storage handler.
    /// </summary>
    public class GoogleDriveStorageHandler : StorageHandler
    {
        private readonly GoogleDriveClient _client;

        /// <summary>
        /// Creates a new instance of the GoogleDrive storage handler.
        /// </summary>
        public GoogleDriveStorageHandler()
        {
            //Create a new Google Drive OAuth2Client.
            _client = new GoogleDriveClient();
        }

        #region Storage Handler Implementation

        public override async Task<bool> DeleteAllRemoteData()
        {
            await Task.Delay(100);
            return true;
        }

        public override async Task<bool> InitializeHandler()
        {
            //First, login.
            bool result = await _client.Login();
            if (!result)
                return false; //Couldn't login.

            //Check that the folder "Applications/Bakkup" exists on the drive. Create it if it does not exist.
            

            return true;
        }

        public override async Task<bool> Sync(List<DirectoryInfo> gameSaveDirs)
        {
            await Task.Delay(100);
            return true;
        }

        #endregion

        #region Google Drive API Helper

        private class GFile
        {
            public GFile()
            {
                Properties = new NameValueCollection();
                Parents = new List<GFile>();
                CreateTime = DateTime.Now;
                ModifyDate = DateTime.Now;
            }

            public string ID { get; set; }

            public string Title { get; set; }

            public DateTime CreateTime { get; set; }

            public DateTime ModifyDate { get; set; }

            public string FileExtension { get; set; }

            public string Md5Checksum { get; set; }

            public long FileSize { get; set; }

            public List<GFile> Parents { get; set; }

            public string OriginalFileName { get; set; }

            public NameValueCollection Properties { get; set; }
        }

        private class GDriveApiHelper
        {
            private readonly GoogleDriveClient _client;

            public GDriveApiHelper(GoogleDriveClient client)
            {
                _client = client;
            }

            //Returns a list of files from the user's Google Drive.
            public async Task<List<GFile>> ListFiles()
            {
                return null;
            }
        }

        #endregion
    }
}
