using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace bakkup
{
    [JsonObject(Title = "MachineSpecificConfig")]
    /// <summary>
    /// Represents settings that are specific to each machine a game is on.
    /// </summary>
    public class MachineSpecificConfig
    {
        [JsonProperty("LocalSaveDirectory")]
        /// <summary>
        /// The path to the root save directory of the game.
        /// </summary>
        public string LocalSaveDirectory { get; set; }

        [JsonProperty("LastModifyTime")]
        /// <summary>
        /// Gets or sets the last time the game data was modified on the current machine.
        /// </summary>
        public DateTime LastModifyTime { get; set; }

        [JsonProperty("ExePath")]
        /// <summary>
        /// Gets or sets the path to the game's executable.
        /// </summary>
        public string ExePath { get; set; }

        [JsonProperty("MachineId")]
        /// <summary>
        /// The unique identifier to set a machine apart from other machines. This is a UUID
        /// generated from the CPU and hard drive IDs.
        /// </summary>
        public string MachineId { get; set; }
    }

    [JsonObject(Title = "GameConfig")]
    /// <summary>
    /// Represents game configuration information for a single game.
    /// </summary>
    public class GameConfig
    {
        [JsonConstructor()]
        /// <summary>
        /// Default constructor.
        /// </summary>
        public GameConfig()
        {
            //Make sure the machine config list is initialized.
            MachineConfigs = new List<MachineSpecificConfig>();
        }

        [JsonProperty("StartupParams")]
        /// <summary>
        /// Gets or sets the optional startup parameters for the game.
        /// </summary>
        public string StartupParams { get; set; }

        [JsonProperty("Name")]
        /// <summary>
        /// The name of the game.
        /// </summary>
        public string Name { get; set; }

        [JsonProperty("MachineConfigs")]
        /// <summary>
        /// Gets or sets an array of machine specific configurations for the game.
        /// </summary>
        public List<MachineSpecificConfig> MachineConfigs { get; set; }
    }

    /// <summary>
    /// Represents a GameConfig.json file. Provides methods for reading and writing this file.
    /// </summary>
    public class GameConfigFile
    {
        /// <summary>
        /// Gets or sets the list of game configurations.
        /// </summary>
        public List<GameConfig> Games { get; set; }
    }
}
