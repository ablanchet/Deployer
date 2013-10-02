using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CK.Core;
using Deployer.Action;
using Deployer.Settings;
using Deployer.Settings.Validity;
using Deployer.Utils;

namespace Deployer.Actions
{
    public class DBSetupAction : IAction
    {
        public IEnumerable<string> PatternMatchers
        {
            get { return new string[] { "db", "db-setup" }; }
        }

        public string Description
        {
            get { return "Run the DBSetup to the configured database"; }
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

        public void CheckSettingsValidity( ISettings settings, ISettingsValidityCollector collector, IList<string> extraParameters, IActivityLogger logger )
        {
            // Check connection string
            if( !string.IsNullOrEmpty( settings.ConnectionString ) )
            {
                using( SqlConnection conn = new SqlConnection( settings.ConnectionString ) )
                {
                    try
                    {
                        conn.Open();
                    }
                    catch( Exception ex )
                    {
                        collector.Add( new Results.Result( Results.ResultLevel.Error, string.Format( "Unable to connect to any server with the given connection string.{2}Exception raised is {0}.{2}Message : {1}", ex.GetType().Name, ex.Message ) ) );
                    }
                }
            }
            else collector.Add( new Results.Result( Results.ResultLevel.Error, "No connection string configured" ) );

            // Check log directory
            if( !string.IsNullOrEmpty( settings.LogDirectory ) )
            {
                if( !Directory.Exists( Path.GetFullPath( settings.LogDirectory ) ) )
                {
                    Directory.CreateDirectory( Path.GetFullPath( settings.LogDirectory ) );
                    collector.Add( new Results.Result( Results.ResultLevel.Warning, "Log directory not found. Automatically created." ) );
                }
            }
            else collector.Add( new Results.Result( Results.ResultLevel.Error, "No log directory configured" ) );

        }

        public IActionResult Run( Runner runner, ISettings settings, IList<string> extraParameters, IActivityLogger logger )
        {
            return new ActionResult( Results.ResultLevel.Warning, "Not implemented yet. TODO !" );
        }
    }
}
