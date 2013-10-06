using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deployer.Settings
{
    public interface ISettings
    {
        /// <summary>
        /// Gets if the settings are just newed or loaded from a source.
        /// </summary>
        bool IsNew { get; }

        /// <summary>
        /// Gets the file path to the source. Can be null if IsNew is true.
        /// </summary>
        string FilePath { get; }

        /// <summary>
        /// Gets the absolute root directory of the configuration. 
        /// This will be used by the dbsetup to prefix the dll search.
        /// </summary>
        string RootAbsoluteDirectory { get; }

        /// <summary>
        /// Gets the path to the backup directory.
        /// Why directory ? Because each backup will produce a backup file, stored in this directory.
        /// </summary>
        string BackupDirectory { get; }

        /// <summary>
        /// Gets the path to the log directory.
        /// Why directory ? Because each action can write logs in 
        /// subdirectories of this directory.
        /// </summary>
        string LogDirectory { get; }

        /// <summary>
        /// Gets the paths the directoy where the dbsetup action will look for DLLs to process.
        /// </summary>
        IReadOnlyCollection<string> DllDirectoryPaths { get; }

        /// <summary>
        /// Gets the assembly names that the dbsetup action will try to find in the DLLs to process.
        /// </summary>
        IReadOnlyCollection<string> AssemblieNamesToProcess { get; }

        /// <summary>
        /// Gets the path to the DBSetup console application. (ie. CK.Deploy.Console.exe in /package)
        /// </summary>
        string DBSetupConsolePath { get; }

        /// <summary>
        /// Gets the connection string to the database that you want to operate.
        /// The backup, restore, dbsetups actions are going to need it.
        /// </summary>
        string ConnectionString { get; }
    }
}
