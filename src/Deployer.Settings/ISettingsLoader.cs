using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CK.Core;

namespace Deployer.Settings
{
    public interface ISettingsLoader
    {
        /// <summary>
        /// Load a settings object from the given filepath.
        /// If something goes wrong, an error will be logged in the given logger.
        /// </summary>
        /// <param name="filepath">File path to the config file</param>
        /// <param name="logger">Activity logger used to check errors and so on</param>
        /// <returns>The loaded settings or null</returns>
        ISettings Load( string filepath, IActivityLogger logger );

        /// <summary>
        /// Save the given settings to their filepath
        /// </summary>
        /// <param name="settings">The settings to save</param>
        void Save( ISettings settings );
    }
}
