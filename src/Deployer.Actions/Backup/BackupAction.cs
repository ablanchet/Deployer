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
using Deployer.Utils;

namespace Deployer.Actions
{
    public class BackupAction : IAction
    {
        public IEnumerable<string> PatternMatchers
        {
            get { return new string[] { "b", "backup" }; }
        }

        public string Description
        {
            get { return "Do a quick backup of the configured database"; }
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

            // Check backup directory
            if( !string.IsNullOrEmpty( settings.BackupDirectory ) )
            {
                if( !Directory.Exists( Path.GetFullPath( settings.BackupDirectory ) ) )
                {
                    Directory.CreateDirectory( Path.GetFullPath( settings.BackupDirectory ) );
                    logger.Warn( "Backup directory not found. Automatically created." );
                }
            }
            else logger.Error( "No backup directory configured" );

        }

        public void Run( Runner runner, ISettings settings, IList<string> extraParameters, IActivityLogger logger )
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

                    using( StreamReader sr = new StreamReader( Assembly.GetExecutingAssembly().GetManifestResourceStream( "Deployer.Actions.Backup.BackupFormat.sql" ) ) )
                    {
                        string sqlFile = sr.ReadToEnd();
                        using( var cmd = conn.CreateCommand() )
                        {
                            string backupFilename = string.Concat( conn.Database, "-", DateTime.Now.ToString( "dd-MM-yyyy HH-mm" ), ".bak" );

                            cmd.CommandText = string.Format( sqlFile, conn.Database, Path.Combine( Path.GetFullPath( settings.BackupDirectory ), backupFilename ) );
                            using( logger.OpenGroup( LogLevel.Info, "Starting backup" ) )
                            {
                                cmd.ExecuteNonQuery();
                            }

                            logger.Info( "Backup finished, check {0} in your backup directory", backupFilename );
                        }
                    }
                }
                catch( Exception ex )
                {
                    logger.Error( ex, "Unable to backup the database." );
                }
            }
        }
    }
}
