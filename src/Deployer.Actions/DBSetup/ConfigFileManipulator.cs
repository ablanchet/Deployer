using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using CK.Core;
using Deployer.Settings;
using Deployer.Utils;
using Deployer.Utils.Rebindings;
using Mono.Cecil;

namespace Deployer.Actions.DBSetup
{
    public class ConfigFileManipulator
    {
        ISettings _settings;
        IActivityLogger _logger;

        public ConfigFileManipulator( ISettings settings, IActivityLogger logger )
        {
            _settings = settings;
            _logger = logger;
        }

        public void ResetAssemblyRebindings( AssemblyNameDefinition assemblyName )
        {
            XDocument xConfig = null;
            string configPath = string.Concat( _settings.DBSetupConsolePath, ".config" );
            if( File.Exists( configPath ) )
            {
                try
                {
                    xConfig = XDocument.Load( configPath );
                }
                catch( Exception ex )
                {
                    _logger.Warn( ex, "Unable to load the configuration file {0}", configPath );
                    _logger.Warn( "Rebuilding a config file from scratch" );
                    CreateConfigurationNode( ref xConfig );
                }
            }
            else
                CreateConfigurationNode( ref xConfig );

            XElement xRuntime = xConfig.Root.Element( "runtime" );
            if( xRuntime == null )
                xRuntime = CreateNodeAndAttach( xConfig.Root, "runtime" );

            XElement xAssemblyBinding = xRuntime.Element( MicrosoftAsmV1.AssemblyBinding );
            if( xAssemblyBinding == null )
                xAssemblyBinding = CreateNodeAndAttach( xRuntime, MicrosoftAsmV1.AssemblyBinding );


            XElement xDependentAssembly = null;
            foreach( var dp in xAssemblyBinding.Elements( MicrosoftAsmV1.DependentAssembly ) )
            {
                var assemblyIdentity = dp.Element( MicrosoftAsmV1.AssemblyIdentity );
                if( assemblyIdentity != null )
                {
                    var nameAttr = assemblyIdentity.Attribute( "name" );
                    if( nameAttr != null && nameAttr.Value == assemblyName.Name )
                    {
                        xDependentAssembly = dp;
                        break;
                    }
                }
            }

            AssemblyBindingElement binding = new AssemblyBindingElement();
            if( xDependentAssembly != null )
            {
                binding.FromXml( xDependentAssembly );
                xDependentAssembly.Remove();
            }

            binding.Name = assemblyName.Name;
            binding.OldVersion = string.Format( "{0}-{1}", Util.EmptyVersion.ToString( 4 ), assemblyName.Version.ToString( 4 ) );
            binding.NewVersion = assemblyName.Version.ToString( 4 );
            binding.PublicKeyToken = BitConverter.ToString( assemblyName.PublicKeyToken ).Replace( "-", string.Empty );

            xDependentAssembly = binding.ToXml();
            xAssemblyBinding.Add( xDependentAssembly );

            xConfig.Save( configPath, SaveOptions.OmitDuplicateNamespaces );
        }

        void CreateConfigurationNode( ref XDocument document )
        {
            document = new XDocument();
            document.Add( new XElement( "configuration" ) );
        }

        XElement CreateNodeAndAttach( XElement parent, XName name )
        {
            XElement element = new XElement( name );
            parent.Add( element );

            return element;
        }
    }
}
