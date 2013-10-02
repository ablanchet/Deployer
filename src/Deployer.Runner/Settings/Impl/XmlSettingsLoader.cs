using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Deployer.Settings.Impl
{
    internal class XmlSettingsLoader : ISettingsLoader
    {
        public const string DefaultConfigurationFileName = "Deployer.config";

        public ISettings Load( string filePath )
        {
            if( string.IsNullOrEmpty( filePath ) && File.Exists( DefaultConfigurationFileName ) )
                filePath = DefaultConfigurationFileName;

            if( !string.IsNullOrEmpty( filePath ) )
            {
                return LoadFromFile( filePath );
            }
            return new XmlSettings();
        }

        public void Save( ISettings settings )
        {
            XmlSettings xmlSettings = settings as XmlSettings;
            if( xmlSettings == null )
                throw new ArgumentException( "XmlSettingsLoader is not able to save other settings than XmlSettings." );

            if( string.IsNullOrEmpty( xmlSettings.FilePath ) )
                xmlSettings.FilePath = DefaultConfigurationFileName;

            using( Stream fileStream = File.OpenWrite( xmlSettings.FilePath ) )
            {
                XmlSerializer x = new XmlSerializer( typeof( XmlSettings ) );
                x.Serialize( fileStream, xmlSettings );
            }
        }

        XmlSettings LoadFromFile( string filePath )
        {
            using( Stream fileStream = File.OpenRead( filePath ) )
            {
                if( fileStream.Length > 0 )
                {
                    XmlSerializer x = new XmlSerializer( typeof( XmlSettings ) );
                    XmlSettings s = (XmlSettings)x.Deserialize( fileStream );
                    s.FilePath = filePath;
                    return s;
                }
                else
                    return new XmlSettings();
            }
        }
    }
}
