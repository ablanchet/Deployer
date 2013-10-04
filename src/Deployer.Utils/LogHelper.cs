using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CK.Core;
using Deployer.Settings;

namespace Deployer.Utils
{
    public static class LogHelper
    {
        public static TextWriter ReplicateIn( IActivityLogger logger, ISettings settings, string directoryName, string filename )
        {
            DefaultActivityLogger dLogger = logger as DefaultActivityLogger;
            if( dLogger != null )
            {
                if( !string.IsNullOrEmpty( settings.LogDirectory ) )
                {
                    string backupLogDirectory = Path.Combine( settings.LogDirectory, directoryName );

                    if( !Directory.Exists( backupLogDirectory ) )
                        Directory.CreateDirectory( backupLogDirectory );

                    string logFilePath = Path.Combine( backupLogDirectory, filename );

                    TextWriter txtWr = File.CreateText( logFilePath );
                    dLogger.Tap.Register( new ActivityLoggerTextWriterSink( txtWr ) );

                    return txtWr;
                }
            }

            return TextWriter.Null;
        }
    }
}
