using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CK.Core;
using Deployer.Settings;

namespace Deployer.Action
{
    /// <summary>
    /// An action is a "thing" that can be executed through the command line. program.exe -help will run the HelpAction class implementing IAction interface.
    /// </summary>
    public interface IAction
    {
        /// <summary>
        /// Gets the description of the action. 
        /// This description will be shown when the user will try to display the program help.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// First step in the action invokation process. 
        /// The action will have to load the settings with the loader, based on the extraParameters. 
        /// If the action do not need particular specific behavior, just returm loader.Load()
        /// </summary>
        /// <param name="loader">The ISettingsLoader that can load settigns</param>
        /// <param name="extraParameters">The extraParameters found in the command line</param>
        /// <param name="logger">The activity logger used to check errors and so on</param>
        /// <returns>Loaded settings</returns>
        ISettings LoadSettings( ISettingsLoader loader, IList<string> extraParameters, IActivityLogger logger );

        /// <summary>
        /// Second step in the action invokation process.
        /// Just check if the settings loaded previously are valid.
        /// For example, the backup action will check if the configured connection string is valid and if the database is reachable.
        /// </summary>
        /// <param name="settings">The previously loaded settings</param>
        /// <param name="extraParameters">The extraParameters found in the command line</param>
        /// <param name="logger">The activity logger used to check errors and so on</param>
        void CheckSettingsValidity( ISettings settings, IList<string> extraParameters, IActivityLogger logger );

        /// <summary>
        /// Third (and final) step in the action invokation process.
        /// This is the method that DO something.
        /// </summary>
        /// <param name="runner">The runner. The instance of the top level object that run actions</param>
        /// <param name="settings">The previous loaded, and valid settings</param>
        /// <param name="extraParameters">The extraParameters found in the command line</param>
        /// <param name="logger">The activity logger used to check errors and so on</param>
        void Run( Runner runner, ISettings settings, IList<string> extraParameters, IActivityLogger logger );

        /// <summary>
        /// Retreave all the sub options to be display in help for this action with all its arguments
        /// </summary>
        /// <returns>All the arguments for this action</returns>
        IEnumerable<SubOptions> GetSubOptions();
    }
}
