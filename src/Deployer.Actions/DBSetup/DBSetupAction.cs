using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CK.Core;
using Deployer.Action;
using Deployer.Settings;
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

        public Settings.ISettings LoadSettings( ISettingsLoader loader, IList<string> extraParameters, IActivityLogger logger )
        {
            string path = null;
            if( extraParameters.Count == 1 ) path = extraParameters[0];
            try
            {
                return loader.Load( path, logger );
            }
            catch( Exception ex )
            {
                logger.Error( ex, "Unable to load configuration" );
            }

            return null;
        }

        public void CheckSettingsValidity( ISettings settings, IList<string> extraParameters, IActivityLogger logger )
        {
            // Check rootdirectorypath
            if( !string.IsNullOrEmpty( settings.RootAbsoluteDirectory ) )
            {
                if( !Directory.Exists( settings.RootAbsoluteDirectory ) )
                {
                    logger.Error( "The absolute root directory does not exists at {0}", settings.RootAbsoluteDirectory );
                }
            }
            else logger.Error( "No absolute root directory configured" );

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
                        logger.Error( ex, "Unable to connect to any server with the given connection string." );
                    }
                }
            }
            else logger.Error( "No connection string configured" );

            // Check log directory
            if( !string.IsNullOrEmpty( settings.LogDirectory ) )
            {
                if( !Directory.Exists( Path.GetFullPath( settings.LogDirectory ) ) )
                {
                    Directory.CreateDirectory( Path.GetFullPath( settings.LogDirectory ) );
                    logger.Warn( "Log directory not found. Automatically created." );
                }
            }
            else logger.Error( "No log directory configured" );

            // Check dbsetup path
            if( !string.IsNullOrEmpty( settings.DBSetupConsolePath ) )
            {
                if( !File.Exists( Path.GetFullPath( settings.DBSetupConsolePath ) ) )
                {
                    logger.Error( "DBSetup console not found at {0}", settings.DBSetupConsolePath );
                }
            }
            else logger.Error( "No dbsetup console application configured" );


            // Check rootdirectorypath
            if( settings.DllDirectoryPaths != null && settings.DllDirectoryPaths.Count > 0 )
            {
                foreach( var p in settings.DllDirectoryPaths.Select( p => Path.GetFullPath( (p) ) ) )
                {
                    if( !Directory.Exists( p ) )
                    {
                        logger.Error( "The dll directory does not exists at {0}", p );
                    }
                }
            }
            else logger.Error( "No dll directory paths configured" );

            // Check AssemblieNamesToProcess
            if( settings.AssemblieNamesToProcess == null || settings.AssemblieNamesToProcess.Count == 0 )
                logger.Error( "No assemblieNames to process configured" );
        }

        public void Run( Runner runner, ISettings settings, IList<string> extraParameters, IActivityLogger logger )
        {
            int innerErrorCount = 0;
            using( LogHelper.ReplicateIn( logger, settings, "DBSetups", string.Concat( "DBSetup-", DateTime.Now.ToString( "dd-MM-yyyy HH-mm" ), ".log" ) ) )
            using( logger.OpenGroup( LogLevel.Info, "DBSetup" ) )
            {
                using( logger.CatchCounter( ( errorCount ) => innerErrorCount = errorCount ) )
                using( logger.OpenGroup( LogLevel.Info, "Backup" ) )
                {
                    runner.RunSpecificAction<BackupAction>( settings, extraParameters );
                }

                if( innerErrorCount == 0 )
                {
                    using( logger.OpenGroup( LogLevel.Info, "DBSetup process" ) )
                    {
                        // TODO : crunch file paths in order to configure the underlying process correctly

                        logger.Warn( "This is a very early version !" );
                        logger.Warn("The various file paths are not preprocessed");
                        logger.Warn("This can be very buggy !" );

                        if( CommandLineHelper.PromptBool( "Are you sure you want to run this buggy command ?" ) )
                        {
                            logger.Info( "OK, has you want" );
                            string commandline = string.Format( "-v2 \"{1}\" \"\" {2} {3} \"{4}\"",
                                settings.DBSetupConsolePath,
                                settings.RootAbsoluteDirectory,
                                string.Join( ";", settings.DllDirectoryPaths.Select( p => '"' + p + '"' ) ),
                                string.Join( ";", settings.AssemblieNamesToProcess.Select( p => '"' + p + '"' ) ),
                                settings.ConnectionString );

                            ProcessStartInfo processStartInfo = new ProcessStartInfo( settings.DBSetupConsolePath, commandline );
                            processStartInfo.RedirectStandardOutput = true;
                            processStartInfo.UseShellExecute = false;
                            processStartInfo.CreateNoWindow = true;

                            Process process = Process.Start( processStartInfo );

                            logger.Info( process.StandardOutput.ReadToEnd() );

                            process.WaitForExit();
                            process.Close();
                        }
                        else
                        {
                            logger.Info( "DBSetup aborted" );
                        }
                    }
                }
            }
        }
    }
}
