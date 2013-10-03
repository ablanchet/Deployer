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
        public IEnumerable<string> PatternMatchers
        {
            get { return new string[] { "d", "display-config" }; }
        }

        public string Description
        {
            get { return "Display the configuration that will be use while run and other operations"; }
        }

        public void CheckSettingsValidity( Settings.ISettings settings, IList<string> extraParameters, IActivityLogger logger )
        {
        }

        public Settings.ISettings LoadSettings( ISettingsLoader loader, IList<string> extraParameters, IActivityLogger logger )
        {
            string path = null;
            if( extraParameters.Count == 1 ) path = extraParameters[0];
            try
            {
                return loader.Load( path );
            }
            catch( Exception ex )
            {
                logger.Error( ex, "Unable to load configuration" );
            }

            return null;
        }

        public void Run( Runner runner, Settings.ISettings settings, IList<string> extraParameters, IActivityLogger logger )
        {
            if( !settings.IsNew )
            {
                Console.WriteLine( "Configuration file path : {0}{1}{0}", Environment.NewLine, Path.GetFullPath( settings.FilePath ) );
                Console.WriteLine( "Backup directory path : {0}{1}{0}", Environment.NewLine, Path.GetFullPath( settings.BackupDirectory ) );
                Console.WriteLine( "Log directory path : {0}{1}{0}", Environment.NewLine, Path.GetFullPath( settings.LogDirectory ) );
                Console.WriteLine( "Root directory : {0}{1}{0}", Environment.NewLine, Path.GetFullPath( settings.RootAbsoluteDirectory ) );
                Console.WriteLine( "Dlls to process : {0}{1}{0}", Environment.NewLine, settings.DllPaths != null && settings.DllPaths.Count > 0 ? string.Join( Environment.NewLine, settings.DllPaths.Select( p => Path.GetFullPath( p ) ) ) : "(none)" );
                Console.WriteLine( "Assemblies to process : {0}{1}{0}", Environment.NewLine, settings.AssembliesToProcess != null && settings.AssembliesToProcess.Count > 0 ? string.Join( Environment.NewLine, settings.AssembliesToProcess ) : "(none)" );
                Console.WriteLine( "Connection string : {0}{1}", Environment.NewLine, settings.ConnectionString );
                DatabaseHelper.TryToConnectToDB( settings.ConnectionString, logger );
            }
            else
                logger.Warn( "No configuration file found. Please run --setup to configure the settings" );
        }
    }
}
