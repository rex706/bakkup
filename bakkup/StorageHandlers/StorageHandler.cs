﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bakkup.Clients;

namespace bakkup.StorageHandlers
{
    /// <summary>
    /// Represents a storage handler that keeps the local game save data in sync with online save data. Should be
    /// extended to handle different online services such as Google Drive, OneDrive, DropBox, etc.
    /// </summary>
    public interface IStorageHandler
    {
        /// <summary>
        /// The handler should perform necessary initialization on the remote storage drive, if necessary.
        /// </summary>
        /// <returns>A value indicating success or failure of the operation.</returns>
        Task<bool> InitializeHandler();

        /// <summary>
        /// Indicates the StorageHandler should perform a full sync. This means the local and remote save game folders
        /// should be updated and match with whatever was last modified.
        /// </summary>
        /// <param name="gameSaveDirs">The list of registered save game directories to work on.</param>
        /// <returns>A value indicating success or failure of the operation.</returns>
        Task<bool> Sync(List<DirectoryInfo> gameSaveDirs);

        /// <summary>
        /// Deletes all game save data off of the remote drive, including the game save folder.
        /// </summary>
        /// <returns>A value indicating success or failure of the operation.</returns>
        Task<bool> DeleteAllRemoteData();

        /// <summary>
        /// Gets the error message of the last message. This should be checked any time a call fails. For the most
        /// part, this just propogates errors from the internal OAuth2Client.
        /// </summary>
        string LastErrorMessage { get; }

        /// <summary>
        /// Gets the last error that occurred. This should be checked any time a call fails. For the most part, this
        /// just propogates errors from the internal OAuth2Client.
        /// </summary>
        OAuthClientResult LastError { get; }
    }
}