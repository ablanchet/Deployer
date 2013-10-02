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
using Deployer.Settings.Validity;
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

        IReadOnlyCollection<string> ISettings.DllPaths
        {
            get { return new ReadOnlyCollection<string>( DllPaths.ToList() ); }
        }

        IReadOnlyCollection<string> ISettings.AssembliesToProcess
        {
            get { return new ReadOnlyCollection<string>( AssembliesToProcess.ToList() ); }
        }

    }

    public class SettingsConfigurator : IAction
    {
        public IEnumerable<string> PatternMatchers
        {
            get { return new string[] { "s", "setup" }; }
        }

        public string Description
        {
            get { return "Configure the application with a nice walkthroug"; }
        }

        public void CheckSettingsValidity( ISettings settings, ISettingsValidityCollector collector, IList<string> extraParameters, IActivityLogger logger )
        {
        }

        public ISettings LoadSettings( ISettingsLoader loader, ISettingsValidityCollector collector, IList<string> extraParameters, IActivityLogger logger )
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

        public IActionResult Run( Runner runner, ISettings settings, IList<string> extraParameters, IActivityLogger logger )
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

                        TryToConnectToDB( b.ConnectionString );

                        editableSettings.ConnectionString = connectionString;
                    }
                    catch( ArgumentException )
                    {
                        using( ConsoleHelper.ScopeForegroundColor( ConsoleColor.Red ) )
                            Console.WriteLine( "The given connection string is invalid" );
                    }
                }

                editableSettings.DllPaths = CommandLineHelper.PromptStringArray( "Enter the dlls paths that you want to use for the dbsetup", IsValidDllPath, settings.IsNew ? (string[])null : settings.DllPaths.ToArray() );
                editableSettings.AssembliesToProcess = CommandLineHelper.PromptStringArray( "Enter the assembly names that you want to load for the dbsetup", null, settings.IsNew ? (string[])null : settings.AssembliesToProcess.ToArray() );



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


                    Console.WriteLine();
                    using( ConsoleHelper.ScopeForegroundColor( ConsoleColor.Green ) )
                    {
                        Console.WriteLine( "Ok, the configuration has been saved to {0}", editableSettings.FilePath );
                    }
                }
                else
                {
                    Console.WriteLine();
                    Console.WriteLine( "Setup is cancelled, nothing will be saved." );
                    Console.ReadLine();
                }
            }

            return SucceedActionResult.Result;
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

        bool IsValidDllPath( string path )
        {
            try
            {
                string ex = Path.GetExtension( path );
                return ex == ".dll" && File.Exists( path );
            }
            catch( Exception )
            {
                return false;
            }
        }
    }
}
