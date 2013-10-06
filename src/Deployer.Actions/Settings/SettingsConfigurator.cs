using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    class EditableSettings : ISettings
    {
        public EditableSettings( ISettings sourceSettings )
        {
            FilePath = sourceSettings.FilePath;
        }

        public bool IsNew { get; set; }

        public string RootAbsoluteDirectory { get; set; }

        public string BackupDirectory { get; set; }

        public string LogDirectory { get; set; }

        public IEnumerable<string> DllPaths { get; set; }

        public IEnumerable<string> AssembliesToProcess { get; set; }

        public string ConnectionString { get; set; }

        public string FilePath { get; set; }

        public string DBSetupConsolePath { get; set; }

        IReadOnlyCollection<string> ISettings.DllDirectoryPaths
        {
            get { return new ReadOnlyCollection<string>( DllPaths.ToList() ); }
        }

        IReadOnlyCollection<string> ISettings.AssemblieNamesToProcess
        {
            get { return new ReadOnlyCollection<string>( AssembliesToProcess.ToList() ); }
        }

    }

    public class SettingsConfigurator : IAction
    {
        public string Description
        {
            get { return "Configure the application with a nice walkthroug"; }
        }

        public void CheckSettingsValidity( ISettings settings, IList<string> extraParameters, IActivityLogger logger )
        {
        }

        public ISettings LoadSettings( ISettingsLoader loader, IList<string> extraParameters, IActivityLogger logger )
        {
            return ConfigHelper.TryLoadCustomPathOrDefault( loader, extraParameters, logger );
        }

        public void Run( Runner runner, ISettings settings, IList<string> extraParameters, IActivityLogger logger )
        {
            if( CommandLineHelper.PromptBool( "Let's start the configuration ?", "yes" ) )
            {
                EditableSettings editableSettings = new EditableSettings( settings );

                editableSettings.RootAbsoluteDirectory = CommandLineHelper.PromptString( "What is the absolute root directory ?", settings.IsNew ? Environment.CurrentDirectory : settings.RootAbsoluteDirectory );
                editableSettings.BackupDirectory = CommandLineHelper.PromptString( "What is the backup directory ?", settings.IsNew ? Path.Combine( Environment.CurrentDirectory, "Backups" ) : settings.BackupDirectory );
                editableSettings.LogDirectory = CommandLineHelper.PromptString( "What is the log directory ?", settings.IsNew ? Path.Combine( Environment.CurrentDirectory, "Logs" ) : settings.LogDirectory );

                bool check = true;
                while( check )
                {
                    try
                    {
                        string connectionString = CommandLineHelper.PromptString( "What is the connection string of the database that you want to apply the dbsetup ?", settings.IsNew ? (string)null : settings.ConnectionString );
                        // check connection string validity
                        SqlConnectionStringBuilder b = new SqlConnectionStringBuilder();
                        b.ConnectionString = connectionString;
                        check = false;

                        DatabaseHelper.TryToConnectToDB( b.ConnectionString, logger );

                        editableSettings.ConnectionString = connectionString;
                    }
                    catch( ArgumentException ex )
                    {
                        logger.Error( ex, "The given connection string is invalid" );
                    }
                }

                editableSettings.DllPaths = CommandLineHelper.PromptStringArray( "Enter the directory path where you want the dbsetup to look for dlls to process", null, settings.IsNew ? (string[])null : settings.DllDirectoryPaths.ToArray() );
                editableSettings.AssembliesToProcess = CommandLineHelper.PromptStringArray( "Enter the assembly names that you want to load for the dbsetup", null, settings.IsNew ? (string[])null : settings.AssemblieNamesToProcess.ToArray() );
                editableSettings.DBSetupConsolePath = CommandLineHelper.PromptString( "Enter the path to the dbsetup console application", settings.IsNew ? (string)null : settings.DBSetupConsolePath );

                if( CommandLineHelper.PromptBool( "Save ?", "yes" ) )
                {
                    if( !settings.IsNew )
                    {
                        if( !CommandLineHelper.PromptBool( string.Format( "Would you like to overwrite the current settings ?" ), "yes" ) )
                        {
                            editableSettings.FilePath = CommandLineHelper.PromptString( "Where do you want to save this configuration ?" );
                        }
                    }

                    runner.UpdateSettings( settings, editableSettings );

                    logger.Info( "Ok, the configuration has been saved" );
                }
                else
                {
                    logger.Warn( "Setup is cancelled, nothing will be saved." );
                }
            }
        }
    }
}
