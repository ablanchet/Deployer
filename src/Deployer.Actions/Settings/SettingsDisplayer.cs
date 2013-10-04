using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CK.Core;
using Deployer.Action;
using Deployer.Settings;
using Deployer.Utils;

namespace Deployer.Actions
{
    public class SettingsDisplayer : IAction
    {
        public string Description
        {
            get { return "Display the configuration that will be use while run and other operations"; }
        }

        public void CheckSettingsValidity( Settings.ISettings settings, IList<string> extraParameters, IActivityLogger logger )
        {
        }

        public Settings.ISettings LoadSettings( ISettingsLoader loader, IList<string> extraParameters, IActivityLogger logger )
        {
            return ConfigHelper.TryLoadCustomPathOrDefault( loader, extraParameters, logger );
        }

        public void Run( Runner runner, Settings.ISettings settings, IList<string> extraParameters, IActivityLogger logger )
        {
            if( !settings.IsNew )
            {
                Console.WriteLine( "Configuration file path : {0}{1}{0}", Environment.NewLine, Path.GetFullPath( settings.FilePath ) );
                Console.WriteLine( "Backup directory path : {0}{1}{0}", Environment.NewLine, Path.GetFullPath( settings.BackupDirectory ) );
                Console.WriteLine( "Log directory path : {0}{1}{0}", Environment.NewLine, Path.GetFullPath( settings.LogDirectory ) );
                Console.WriteLine( "Root directory : {0}{1}{0}", Environment.NewLine, Path.GetFullPath( settings.RootAbsoluteDirectory ) );
                Console.WriteLine( "Dlls to process : {0}{1}{0}", Environment.NewLine, settings.DllDirectoryPaths != null && settings.DllDirectoryPaths.Count > 0 ? string.Join( Environment.NewLine, settings.DllDirectoryPaths.Select( p => Path.GetFullPath( p ) ) ) : "(none)" );
                Console.WriteLine( "Assemblies to process : {0}{1}{0}", Environment.NewLine, settings.AssemblieNamesToProcess != null && settings.AssemblieNamesToProcess.Count > 0 ? string.Join( Environment.NewLine, settings.AssemblieNamesToProcess ) : "(none)" );
                Console.WriteLine( "Connection string : {0}{1}", Environment.NewLine, settings.ConnectionString );
                DatabaseHelper.TryToConnectToDB( settings.ConnectionString, logger );
            }
            else
                logger.Warn( "No configuration file found. Please run -? to show usage and see how you can set the configuration." );
        }
    }
}
