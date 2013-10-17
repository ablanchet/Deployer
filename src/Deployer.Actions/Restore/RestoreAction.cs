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
        public string Description
        {
            get { return "Restore the last backup file to the configured database"; }
        }

        public ISettings LoadSettings( ISettingsLoader loader, IList<string> extraParameters, IActivityLogger logger )
        {
            return ConfigHelper.TryLoadCustomPathOrDefault( loader, extraParameters, logger );
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
            string formatedDate = DateTime.Now.ToFileFormatString();

            FileInfo backupFile = null;
            DirectoryInfo backupDirectory = new DirectoryInfo( Path.GetFullPath( settings.BackupDirectory ) );

            // find the last backup
            using( logger.OpenGroup( LogLevel.Info, "Looking for the last written backup file" ) )
            {
                using( logger.OpenGroup( LogLevel.Info, "Available backup files" ) )
                {
                    foreach( var bak in backupDirectory.EnumerateFiles( "*.bak" ) )
                    {
                        logger.Info( bak.Name );
                        if( backupFile == null || bak.LastWriteTimeUtc > backupFile.LastWriteTimeUtc )
                            backupFile = bak;
                    }
                }

                using( logger.OpenGroup( LogLevel.Warn, "Last backup file found. Here are some details :" ) )
                {
                    logger.Warn( "Filename : {0}", backupFile.Name );
                    logger.Warn( "Creation date : {0}", backupFile.CreationTime );
                    logger.Warn( "Size : {0} mo", backupFile.Length / 1024 / 1024 );
                }
            }

            if( CommandLineHelper.PromptBool( "Are you sure you want to restore your database ? This cannot be undone !" ) )
            {
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
            else
            {
                logger.Info( "Restore aborted" );
            }
        }
    }
}
