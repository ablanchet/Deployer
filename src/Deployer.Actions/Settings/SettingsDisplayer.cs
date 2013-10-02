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
using Deployer.Settings.Validity;
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

        public void CheckSettingsValidity( Settings.ISettings settings, ISettingsValidityCollector collector, IList<string> extraParameters, IActivityLogger logger )
        {
        }

        public Settings.ISettings LoadSettings( ISettingsLoader loader, ISettingsValidityCollector collector, IList<string> extraParameters, IActivityLogger logger )
        {
            string path = null;
            if( extraParameters.Count == 1 ) path = extraParameters[0];
            try
            {
                return loader.Load( path );
            }
            catch
            {
                collector.Add( new Results.Result( Results.ResultLevel.Error, "Unable to load configuration" ) );
            }

            return null;
        }

        public IActionResult Run( Runner runner, Settings.ISettings settings, IList<string> extraParameters, IActivityLogger logger )
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
                TryToConnectToDB( settings.ConnectionString );

                return SucceedActionResult.Result;
            }

            return new ActionResult( Results.ResultLevel.Warning, "No configuration file found. Please run --setup to configure the settings" );
        }


        bool TryToConnectToDB( string connectionString )
        {
            using( SqlConnection conn = new SqlConnection( connectionString ) )
            {
                try
                {
                    conn.Open();
                    using( ConsoleHelper.ScopeForegroundColor( ConsoleColor.Green ) )
                    {
                        Console.WriteLine( "Test connection succeeded. The database {0} is reachable", conn.Database );
                    }

                    return true;
                }
                catch( Exception ex )
                {
                    using( ConsoleHelper.ScopeForegroundColor( ConsoleColor.Yellow ) )
                    {
                        Console.WriteLine( "Unable to connect to any server with the given connection string.{2}Exception raised is {0}.{2}Message : {1}", ex.GetType().Name, ex.Message, Environment.NewLine );
                    }
                    return false;
                }
            }
        }
    }
}
