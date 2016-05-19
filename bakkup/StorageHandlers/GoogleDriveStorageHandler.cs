using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using bakkup.Clients;
using bakkup.Properties;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Pkix;

namespace bakkup.StorageHandlers
{
    /// <summary>
    /// Represents a Google Drive storage handler.
    /// </summary>
    public class GoogleDriveStorageHandler : IStorageHandler
    {
        private readonly GoogleDriveClient _client;
        private readonly GDriveApiHelper _api;

        /// <summary>
        /// Creates a new instance of the GoogleDrive storage handler.
        /// </summary>
        public GoogleDriveStorageHandler()
        {
            //Create a new Google Drive OAuth2Client.
            _client = new GoogleDriveClient();
            _api = new GDriveApiHelper(_client);
            LastErrorMessage = "Operation Completed Successfully";
        }

        //TODO: Start implementing the IStorageHandler interface for Google Drive.
        #region Storage Handler Implementation

        public async Task<bool> InitializeHandler()
        {
            //First, login.
            var result = await _client.Login();
            if (!result)
            {
                //Couldn't login. Record the last error messages of the client and store in
                //the handler.
                LastError = _client.LastError;
                LastErrorMessage = _client.LastErrorMessage;
                return false; 
            }
                

            //Check that the Bakkup folder exists on the drive. Create it if it does not exist.
            var fileList = await _api.ListRootFiles();
            if (_client.LastError == OAuthClientResult.Success && fileList == null)
            {
                //Parse error.
                LastError = OAuthClientResult.Parser;
                LastErrorMessage = "Error parsing returned data when searching for Bakkup folder.";
                return false;
            }
            if (_client.LastError != OAuthClientResult.Success)
            {
                //Some other error.
                LastError = _client.LastError;
                LastErrorMessage = _client.LastErrorMessage;
                return false;
            }

            var bakkupFolder = from file in fileList
                               where file.Name == Settings.Default.RemoteBakkupFolderName && file.IsFolder
                               select file;
            if (!bakkupFolder.Any())
            {
                //The Bakkup folder does not exist. Create it now.
                var remoteBackupFolder = await
                    _api.CreateFolder(Settings.Default.RemoteBakkupFolderName, Resources.RemoteBakkupFolderDescription,
                        "root");
                if (_client.LastError == OAuthClientResult.Success && remoteBackupFolder == null)
                {
                    //Parse error.
                    LastError = OAuthClientResult.Parser;
                    LastErrorMessage = "Error parsing returned data when searching for Bakkup folder.";
                    return false;
                }
                if (_client.LastError != OAuthClientResult.Success)
                {
                    //Some other error.
                    LastError = _client.LastError;
                    LastErrorMessage = _client.LastErrorMessage;
                    return false;
                }
            }

            LastError = OAuthClientResult.Success;
            LastErrorMessage = "Operation Completed Successfully";
            return true;
        }

        public async Task<bool> Sync()
        {
            throw new NotImplementedException();
        }

        public async Task<bool> Sync(GameConfig game)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> DeleteAllRemoteData()
        {
            throw new NotImplementedException();
        }

        public async Task<List<GameConfig>> RetrieveGameConfigs()
        {
            throw new NotImplementedException();
        }

        public async Task<bool> AddNewGame(GameConfig game)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> EditGame(GameConfig oldGameConfig, GameConfig newGameConfig)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> DeleteGame(GameConfig game)
        {
            throw new NotImplementedException();
        }

        //Method for testing. Remove once folder creation is verified to work.
        public async Task<bool> CreateRemoteTestFolder()
        {
            //Creates the folder "Bakkup" in the root Google Drive folder.
            var result = await _api.CreateFolder("Test Folder", "A test folder created from the Bakkup app.", "root");
            if (_client.LastError == OAuthClientResult.Success && result == null)
            {
                //Parsing error.
                LastError = OAuthClientResult.Parser;
                LastErrorMessage = "Error parsing returned data when creating a folder.";
                return false;
            }
            if (_client.LastError != OAuthClientResult.Success)
            {
                LastError = _client.LastError;
                LastErrorMessage = _client.LastErrorMessage;
                return false;
            }

            LastError = OAuthClientResult.Success;
            LastErrorMessage = "Operation completed successfully";
            return true;
        }

        //Method for testing. Remove once file uploading is verified to work. Note file should be no bigger than
        //5 MB.
        public async Task<bool> UploadSmallTestFile(string fileName, Stream fileStream, IProgress<int> progress)
        {
            var result = await _api.UploadFile(new GFile() {Name = fileName}, fileStream, progress);
            if (_client.LastError == OAuthClientResult.Success && result == null)
            {
                //Parsing error.
                LastError = OAuthClientResult.Parser;
                LastErrorMessage = "Error parsing returned data when creating a folder.";
                return false;
            }
            if (_client.LastError != OAuthClientResult.Success)
            {
                LastError = _client.LastError;
                LastErrorMessage = _client.LastErrorMessage;
                return false;
            }

            LastError = OAuthClientResult.Success;
            LastErrorMessage = "Operation completed successfully";
            return true;
        }

        
        public OAuthClientResult LastError { get; private set; }

        public string LastErrorMessage { get; private set; }

        public string ProviderName => "Google Drive";

        public StorageProviders ProviderType => StorageProviders.GoogleDrive;

        #endregion

        #region Google Drive API Helper

        /// <summary>
        /// Represents Google Drive file metadata. Note this does not implement all metadata.
        /// </summary>
        private class GFile
        {
            public GFile()
            {
                Properties = new NameValueCollection();
                Parents = new List<string>();
                CreateTime = DateTime.Now;
                ModifyDate = DateTime.Now;
            }

            public string ID { get; set; }

            public string Name { get; set; }

            public string Description { get; set; }

            public DateTime CreateTime { get; set; }

            public DateTime ModifyDate { get; set; }

            public string FileExtension { get; set; }

            public string Md5Checksum { get; set; }

            public long FileSize { get; set; }

            public List<string> Parents { get; set; }

            public string OriginalFileName { get; set; }
           
            public NameValueCollection Properties { get; set; }

            public string MimeType { get; set; }

            public bool IsFolder { get; set; }

            public static GFile FromJObject(JObject fileObj)
            {
                var file = new GFile();
                JToken holderValue = null;
                if(fileObj.TryGetValue("id", out holderValue))
                    file.ID = (string)holderValue;
                if (fileObj.TryGetValue("name", out holderValue))
                    file.Name = (string)holderValue;
                if (fileObj.TryGetValue("mimeType", out holderValue))
                    file.MimeType = (string)holderValue;
                //If the mime type is "application/vnd.google-apps.folder", this is a folder.
                file.IsFolder = file.MimeType == "application/vnd.google-apps.folder";
                if (fileObj.TryGetValue("createdTime", out holderValue))
                    file.CreateTime = (DateTime)holderValue;
                if (fileObj.TryGetValue("modifiedTime", out holderValue))
                    file.ModifyDate = (DateTime)holderValue;
                if (fileObj.TryGetValue("fileExtension", out holderValue))
                    file.FileExtension = (string)holderValue;
                if (fileObj.TryGetValue("md5Checksum", out holderValue))
                    file.Md5Checksum = (string)holderValue;
                if (fileObj.TryGetValue("size", out holderValue))
                    file.FileSize = (long)holderValue;
                if(fileObj.TryGetValue("description", out holderValue))
                    file.Description = (string)holderValue;
                if (fileObj.TryGetValue("originalFilename", out holderValue))
                    file.OriginalFileName = (string)holderValue;
                //The parents are really "Parent" types, but the information stored in the "Parent" type is not
                //necessary. Just loop through the array, and for each object, pull out the ID.
                if (fileObj.TryGetValue("parents", out holderValue))
                {
                    foreach (var parentObj in (JArray)holderValue)
                        file.Parents.Add((string)parentObj); //Store the ID field.
                }
                //Like parents, the "Property" object is also a type, but some information for the "Property" type
                //is not necessary. Just loop through the array, and for each object, pull out of the key/value.
                if (fileObj.TryGetValue("properties", out holderValue))
                {
                    //Store each property as a key value pair.
                    foreach (var propertyObj in (JArray)holderValue)
                        file.Properties.Add((string)propertyObj["key"], (string)propertyObj["value"]); 
                }
                

                return file;
            }

            public static JObject ToJObject(GFile file)
            {
                var rootObj = new JObject();
                rootObj["name"] = file.Name;
                if (!string.IsNullOrEmpty(file.Description))
                    rootObj["description"] = file.Description;
                if (!string.IsNullOrEmpty(file.ID))
                    rootObj["id"] = file.ID;
                if (!string.IsNullOrEmpty(file.MimeType))
                    rootObj["mimeType"] = file.MimeType;
                if (file.Parents != null && file.Parents.Count > 0)
                {
                    var parentIDs = new JArray();
                    foreach (var parentID in file.Parents)
                        parentIDs.Add(new JObject() { ["id"] = parentID });
                    rootObj["parents"] = parentIDs;
                }
                if (file.Properties != null && file.Properties.Count > 0)
                {
                    var properties = new JArray();
                    foreach (var key in file.Properties.Keys)
                        properties.Add(new JObject() { ["key"] = (string)key, ["value"] = file.Properties[(string)key] });
                    rootObj["properties"] = properties;
                }

                return rootObj;
            }

            public static List<GFile> FromJArray(JArray arrObj)
            {
                //Loop through each object in this array. Each object is a file.
                var files = new List<GFile>();
                foreach (var fileObj in arrObj)
                {
                    files.Add(GFile.FromJObject((JObject)fileObj));
                }

                return files;
            }

            //Gets the list of fields that should be returned any time file metadata is requested from Google Drive.
            public static string FileMetadataFields()
            {
                //The syntax is "fields=files(field1, field2, etc)" This will be concatenated with the fields= part.
                return "name,id,mimeType,description,parents,appProperties,createdTime,modifiedTime," + 
                    "originalFilename,fileExtension,md5Checksum,size";
            }

            //For some unknown reason, there are two formats to specify the fields to return in a query, and it seems
            //different API calls need different ways to do this. This is the second way to specify the list of fields
            //to return.
            public static string FileMetadataFieldsParenthesis()
            {
                return "files(" + FileMetadataFields() + ")";
            }
        }

        //Acts as a very basic implementation of a Google Drive API REST client. Only contains what is necessary for
        //this app to function.
        private class GDriveApiHelper
        {
            private readonly GoogleDriveClient _client;

            public GDriveApiHelper(GoogleDriveClient client)
            {
                _client = client;
            }

            //Returns a list of files from the root folder of the user's Google Drive.
            public async Task<List<GFile>> ListRootFiles()
            {
                //List files in root and ignore files in trash.
                return await ListFiles("'root' in parents and trashed=false");
            } 

            //Returns a list of files from the user's Google Drive.
            public async Task<List<GFile>> ListFiles(string query)
            {
                //Create the url that is used for listing files. Note that FileMetadataFieldsParenthesis is used
                //instead of FileMetadataFields. 
                string url =
                    "https://www.googleapis.com/drive/v3/files?spaces=drive&pageSize=200&trashed=false"
                    + "&fields=" + GFile.FileMetadataFieldsParenthesis() + "&q=";
                //Url encode the query part of the url.
                url += HttpUtility.UrlEncode(query);

                //Make the authorized request for data.
                var result = await _client.SendAuthorizedRequestMessage(url, HttpMethod.Get);
                if (_client.LastError != OAuthClientResult.Success)
                    return null;

                //The result string should be a large JSON document. Use the parser helper methods.
                try
                {
                    var rootObj = JObject.Parse(result);
                    //"files" is the object we want here. It is an array of GFile objects.
                    return GFile.FromJArray((JArray)rootObj["files"]);
                }
                catch (Exception)
                {
                    //Just return null. The only possible explanation for error is parsing failed for some reason.
                    return null;
                }
            }

            //Creates a folder on Google Drive, and returns a GFile representing the folder.
            //Set parentFolderId to "root" to specify the root folder as the parent.
            public async Task<GFile> CreateFolder(string name, string description, string parentFolderId)
            {
                //Construct a request message to send.
                HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, 
                    "https://www.googleapis.com/drive/v3/files?fields=" + GFile.FileMetadataFields());
                //The parameters for the folder must be provided in the message body as JSON.
                JObject folderData = new JObject();
                folderData["name"] = name;
                folderData["description"] = description;
                folderData["mimeType"] = "application/vnd.google-apps.folder";
                //The following makes JSON that looks like this. [{"id": "parentFolderId"}]
                JArray parentArr = new JArray();
                parentArr.Add(new JObject() {["id"] = parentFolderId});
                folderData["parents"] = parentArr;
                //Convert the JSON to a string.
                string json = folderData.ToString();

                //Create the content.
                StringContent content = new StringContent(json, Encoding.UTF8);
                content.Headers.ContentLength = Encoding.UTF8.GetByteCount(json);
                content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json; charset=UTF-8");
                message.Content = content;

                //Send the create folder command.
                var result = await _client.SendAuthorizedRequestMessage(message);
                if (_client.LastError != OAuthClientResult.Success)
                    return null;

                //Try to parse the returned data.
                try
                {
                    return GFile.FromJObject(JObject.Parse(result));
                }
                catch (Exception)
                {
                    //Just return null. The error at this point is a parsing issue.
                    return null;
                }
            }

            //Create an empty file on Google Drive, and returns a GFile representing the file.
            //Note that this does not upload any data, only creates an empty file.
            public async Task<GFile> CreateFile(GFile fileMetadata)
            {
                //Create the message to send.
                HttpRequestMessage message = new HttpRequestMessage();
                //Use the metadata only url.
                message.RequestUri = new Uri("https://www.googleapis.com/drive/v3/files?fields=" 
                    + GFile.FileMetadataFields());
                message.Method = HttpMethod.Post;
                
                //Create JSON that represents the Google Drive file metadata.
                var jsonText = GFile.ToJObject(fileMetadata).ToString();

                //Create the content using the file metadata, converted to a JSON object.
                StringContent content = new StringContent(jsonText, Encoding.UTF8);
                content.Headers.ContentLength = Encoding.UTF8.GetByteCount(jsonText);
                content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json; charset=UTF-8");
                
                //Send the create file command.
                var result = await _client.SendAuthorizedRequestMessage(message);
                if (_client.LastError != OAuthClientResult.Success)
                    return null;

                try
                {
                    return GFile.FromJObject(JObject.Parse(result));
                }
                catch (Exception)
                {
                    //Parsing issue.
                    return null;
                }
            }

            //Creates a file on Google Drive and uses the specified stream to fill it with data.
            //This should be used for data that is 5MB or less.
            public async Task<GFile> UploadFile(GFile fileMetadata, Stream fileData, IProgress<int> progress)
            {
                //Create the message to send.
                HttpRequestMessage message = new HttpRequestMessage();
                message.Method = HttpMethod.Post;
                //Use the content upload url.
                message.RequestUri = new Uri("https://www.googleapis.com/upload/drive/v3/files?uploadType=multipart" 
                    + "&fields=" + GFile.FileMetadataFields());

                //Create JSON that represents the Google Drive file metadata.
                var jsonText = GFile.ToJObject(fileMetadata).ToString();
    
                //JSON file metadata content part.
                var metadataContent = new StringContent(jsonText, Encoding.UTF8);
                metadataContent.Headers.ContentLength = Encoding.UTF8.GetByteCount(jsonText);
                metadataContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json; charset=UTF-8");

                //Streaming file bytes content.
                var fileContent = new StreamContent(fileData);
                fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/zip");

                //Create the content for the HTTP request. This content must be multipart content, where the first
                //part is the file metadata JSON, split it using the boundary string "content_separator", and then
                //the next part is the data in the stream. Note I have no idea what the subtype parameter is supposed
                //to do. generic_subtype is just a random value, and it works, so I'll leave it like that.
                var mainContent = new MultipartContent("generic_subtype", "content_separator");
                //Add the metadata.
                mainContent.Add(metadataContent);
                //Add the file data.
                mainContent.Add(fileContent);
                //Specify the content type. Notice the boundary specified in the constructor should also be specified
                //here as well.
                mainContent.Headers.ContentType = 
                    MediaTypeHeaderValue.Parse("multipart/related; boundary=content_separator");            

                //Add the multipart content to the actual message.
                message.Content = mainContent;

                //Attempt to upload the file.
                var result = await _client.SendAuthorizedRequestMessage(message, progress);
                if (_client.LastError != OAuthClientResult.Success)
                    return null;

                try
                {
                    return GFile.FromJObject(JObject.Parse(result));
                }
                catch (Exception)
                {
                    //An exception means parsing failed for some reason. Return null.
                    return null;
                }
            }
        }

        #endregion
    }
}
