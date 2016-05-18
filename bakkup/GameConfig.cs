using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bakkup
{
    /// <summary>
    /// Represents game configuration information for a single game.
    /// </summary>
    public class GameConfig
    {
        /// <summary>
        /// Represents settings that are specific to each machine a game is on.
        /// </summary>
        public class MachineSpecificConfig
        {
            /// <summary>
            /// The path to the root save directory of the game.
            /// </summary>
            public string LocalSaveDirectory { get; set; }
            
            /// <summary>
            /// Gets or sets the last time the game data was modified on the current machine.
            /// </summary>
            public DateTime LastModifyTime { get; set; }

            /// <summary>
            /// Gets or sets the path to the game's executable.
            /// </summary>
            public string ExePath { get; set; }

            /// <summary>
            /// The unique identifier to set a machine apart from other machines. This is a UUID
            /// generated from the CPU and hard drive IDs.
            /// </summary>
            public string MachineId { get; set; }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public GameConfig()
        {
            //Make sure the machine config list is initialized.
            MachineConfigs = new List<MachineSpecificConfig>();
        }

        /// <summary>
        /// Gets or sets the optional startup parameters for the game.
        /// </summary>
        public string StartupParams { get; set; }

        /// <summary>
        /// The name of the game.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the list of machine specific configurations for the game.
        /// </summary>
        public List<MachineSpecificConfig> MachineConfigs { get; set; } 
    }

    /// <summary>
    /// Represents a GameConfig.json file. Provides methods for reading and writing this file.
    /// </summary>
    public class GameConfigFile
    {
        public static List<GameConfig> FromFile()
        {
            
        } 
    }
}
