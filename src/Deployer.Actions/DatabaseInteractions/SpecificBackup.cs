using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CK.Core;
using Deployer.Settings;

namespace Deployer.Actions
{
    public class SpecificBackup
    {
        public const string SpecificNamesDirectory = "WithSpecificNames";
        ISettings _settings;
        string _specificName;

        public SpecificBackup( ISettings settings, string specificName )
        {
            _settings = settings;
            _specificName = specificName;

            if( !Directory.Exists( Path.Combine( settings.BackupDirectory, SpecificNamesDirectory ) ) )
                Directory.CreateDirectory( Path.Combine( settings.BackupDirectory, SpecificNamesDirectory ) );
        }

        FileInfo _backupFile;
        public FileInfo BackupFile
        {
            get
            {
                if( _backupFile == null )
                {
                    foreach( var fullFilePath in Directory.EnumerateFiles( Path.Combine( _settings.BackupDirectory, SpecificNamesDirectory ) ) )
                    {
                        string f = Path.GetFileName( fullFilePath );

                        if( f == _specificName || ParseComplexName( f ) )
                            return _backupFile = new FileInfo( fullFilePath );
                    }
                }
                return _backupFile;
            }
        }

        bool ParseComplexName( string filename )
        {
            string tail = filename
                            .Replace( _specificName, null )
                            .Replace( "-", null )
                            .Replace( " ", null )
                            .Replace( ".bak", null );

            return Regex.IsMatch( tail, @"^\d*$" );
        }
    }
}
