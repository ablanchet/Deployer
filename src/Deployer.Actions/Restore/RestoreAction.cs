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
using Deployer.Utils;

namespace Deployer.Actions
{
    public class RestoreAction : IAction
    {
        public IEnumerable<string> PatternMatchers
        {
            get { return new string[] { "r", "restore" }; }
        }

        public string Description
        {
            get { return "Restore the last backup file to the configured database"; }
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

        public void CheckSettingsValidity( ISettings settings, IList<string> extraParameters, IActivityLogger logger )
        {
            // Check connection string
            if( !string.IsNullOrEmpty( settings.ConnectionString ) )
            {
                DatabaseHelper.TryToConnectToDB( settings.ConnectionString, logger );
            }
            else logger.Error( "No connection string configured" );

            // Check backup directory
            if( !string.IsNullOrEmpty( settings.BackupDirectory ) )
            {
                if( Directory.Exists( Path.GetFullPath( settings.BackupDirectory ) ) )
                {
                    if( !Directory.EnumerateFiles( Path.GetFullPath( settings.BackupDirectory ), "*.bak" ).Any() )
                        logger.Error( "The backup directory is empty" );
                }
                else logger.Error( "The backup directory does not exist" );
            }
            else logger.Error( "No backup directory configured" );
        }

        public void Run( Runner runner, ISettings settings, IList<string> extraParameters, IActivityLogger logger )
        {
            string formatedDate = DateTime.Now.ToString( "dd-MM-yyyy HH-mm" );

            using( LogHelper.ReplicateIn( logger, settings, "Restores", string.Concat( "Restore-", formatedDate, ".log" ) ) )
            {
                using( SqlConnection conn = new SqlConnection( settings.ConnectionString ) )
                {
                    try
                    {
                        conn.Open();
                        conn.InfoMessage += ( o, e ) =>
                        {
                            logger.Info( e.Message );
                        };

                        using( StreamReader sr = new StreamReader( Assembly.GetExecutingAssembly().GetManifestResourceStream( "Deployer.Actions.Restore.RestoreFormat.sql" ) ) )
                        {
                            string sqlFile = sr.ReadToEnd();
                            using( var cmd = conn.CreateCommand() )
                            {
                                FileInfo backupFile = null;

                                DirectoryInfo backupDirectory = new DirectoryInfo( Path.GetFullPath( settings.BackupDirectory ) );

                                // find the last backup
                                foreach( var bak in backupDirectory.EnumerateFiles( "*.bak" ) )
                                {
                                    if( backupFile == null || bak.LastWriteTimeUtc > backupFile.LastWriteTimeUtc )
                                        backupFile = bak;
                                }

                                logger.Info( "Backup file to restore : {0}", backupFile.Name );

                                cmd.CommandText = string.Format( sqlFile, conn.Database, Path.Combine( Path.GetFullPath( settings.BackupDirectory ), backupFile.FullName ) );
                                using( logger.OpenGroup( LogLevel.Info, "Starting restore of {0}", conn.Database ) )
                                {
                                    cmd.ExecuteNonQuery();
                                }

                                logger.Info( "Restore finished" );
                            }
                        }
                    }
                    catch( Exception ex )
                    {
                        logger.Error( ex, "Unable to restore the database." );
                    }
                }
            }
        }
    }
}
