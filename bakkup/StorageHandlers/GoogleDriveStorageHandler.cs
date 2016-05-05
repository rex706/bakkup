using System;
using System.Collections.Generic;
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
        private GoogleDriveClient _client;

        public GoogleDriveStorageHandler()
        {
            //Create a new Google Drive OAuth2Client.
            _client = new GoogleDriveClient();
           
        }

        public override async Task<bool> DeleteAllRemoteData()
        {
            await Task.Delay(100);
            return true;
        }

        public override async Task<bool> InitializeHandler()
        {
            await Task.Delay(100);
            return true;
        }

        public override async Task<bool> Sync(List<DirectoryInfo> gameSaveDirs)
        {
            await Task.Delay(100);
            return true;
        }
    }
}
