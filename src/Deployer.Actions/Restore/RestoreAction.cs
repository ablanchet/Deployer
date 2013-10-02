﻿using System;
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

            return loader.Load( path );
        }

        public void CheckSettingsValidity( ISettings settings, ISettingsValidityCollector collector, IActivityLogger logger )
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

            // Check backup director
            if( !string.IsNullOrEmpty( settings.BackupDirectory ) )
            {
                if( Directory.Exists( Path.GetFullPath( settings.BackupDirectory ) ) )
                {
                    if( !Directory.EnumerateFiles( Path.GetFullPath( settings.BackupDirectory ), "*.bak" ).Any() )
                        collector.Add( new Results.Result( Results.ResultLevel.Error, "The backup directory is empty" ) );
                }
                else collector.Add( new Results.Result( Results.ResultLevel.Error, "The backup directory does not exist" ) );
            }
            else collector.Add( new Results.Result( Results.ResultLevel.Error, "No backup directory configured" ) );

        }

        public IActionResult Run( Runner runner, ISettings settings, IList<string> extraParameters, IActivityLogger logger )
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

                    using( StreamReader sr = new StreamReader( Assembly.GetExecutingAssembly().GetManifestResourceStream( "CommandRunner.Actions.Restore.RestoreFormat.sql" ) ) )
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

                            cmd.CommandText = string.Format( sqlFile, conn.Database, Path.Combine( Path.GetFullPath( settings.BackupDirectory ), backupFile.FullName ) );
                            using( logger.OpenGroup( LogLevel.Info, "Starting restore" ) )
                            {
                                cmd.ExecuteNonQuery();
                            }

                            return new ActionResult( Results.ResultLevel.Success, string.Format( "Restore finished" ) );
                        }
                    }
                }
                catch( Exception ex )
                {
                    return new ActionResult( Results.ResultLevel.Error, string.Format( "Unable to restore the database. {2}Exception raised is {0}.{2}Message : {1}", ex.GetType().Name, ex.Message, Environment.NewLine ) );
                }
            }
        }
    }
}