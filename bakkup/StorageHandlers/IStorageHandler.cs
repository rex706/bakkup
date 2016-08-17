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
    /// Represents the different supported storage handlers.
    /// </summary>
    public enum StorageProviders
    {
        /// <summary>
        /// An invalid value specifying no storage handler.
        /// </summary>
        None = 0,
        /// <summary>
        /// Specifies the Google Drive storage handler.
        /// </summary>
        GoogleDrive = 1,
        /// <summary>
        /// Specifies the OneDrive storage handler.
        /// </summary>
        OneDrive = 2,
        /// <summary>
        /// Specifies the DropBox storage handler.
        /// </summary>
        DropBox = 3
    }

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
        /// Indicates the StorageHandler should perform a full sync of all games. This means the local and 
        /// remote save game data should be updated to match whatever was last modified.
        /// </summary>
        /// <returns>A value indicating success or failure of the operation.</returns>
        Task<bool> Sync();

        /// <summary>
        /// Indicates the StorageHandler should perform a full sync of the given game. This means the local
        /// and remote save game data should be updated to match whatever was last modified.
        /// </summary>
        /// <param name="game">The game to sync the save data of.</param>
        /// <returns>A value indicating success or failure of the operation.</returns>
        Task<bool> Sync(GameConfig game);

        /// <summary>
        /// Deletes all game save data off of the remote drive, including the game save folder.
        /// </summary>
        /// <returns>A value indicating success or failure of the operation.</returns>
        Task<bool> DeleteAllRemoteData();

        /// <summary>
        /// Retrieves the list of game configs stored in the GameConfigs.json file in the root of the
        /// bakkup application folder. Implementers of this method should store a copy of the GameConfig list.
        /// </summary>
        /// <returns>A game config file containing the list of games.</returns>
        Task<List<GameConfig>> RetrieveGameConfigs();

        /// <summary>
        /// Adds a new game to the storage handler and immediately uploads the game data.
        /// </summary>
        /// <param name="game">The game to add.</param>
        /// <returns>A value indicating success or failure of the operation.</returns>
        Task<bool> AddNewGame(GameConfig game);

        /// <summary>
        /// Edits the given game in the storage handler. Performs syncing if necessary.
        /// </summary>
        /// <param name="oldGameConfig">The old game configuration.</param>
        /// <param name="newGameConfig">The new game configuration.</param>
        /// <returns>A value indicating success or failure of the operation.</returns>
        Task<bool> EditGame(GameConfig oldGameConfig, GameConfig newGameConfig);

        /// <summary>
        /// Deletes the given game in the storage handler.
        /// </summary>
        /// <param name="game">The game to delete.</param>
        /// <returns>A value indicating success or failure of the operation.</returns>
        Task<bool> DeleteGame(GameConfig game);

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

        /// <summary>
        /// Gets the provider name.
        /// </summary>
        string ProviderName { get; }

        /// <summary>
        /// Gets the type of the provider.
        /// </summary>
        StorageProviders ProviderType { get; }
    }
}
