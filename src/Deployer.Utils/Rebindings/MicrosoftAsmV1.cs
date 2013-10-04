using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Deployer.Utils.Rebindings
{
    public static class MicrosoftAsmV1
    {
        public static readonly string Namespace = "urn:schemas-microsoft-com:asm.v1";

        public static readonly XName AssemblyBinding = XName.Get( "assemblyBinding", Namespace );
        public static readonly XName DependentAssembly = XName.Get( "dependentAssembly", Namespace );
        public static readonly XName AssemblyIdentity = XName.Get( "assemblyIdentity", Namespace );
        
        public static readonly XName Name = XName.Get( "name" );
        public static readonly XName PublicKeyToken = XName.Get( "publicKeyToken" );
        public static readonly XName Culture = XName.Get( "culture" );
        public static readonly XName ProcessorArchitecture = XName.Get( "processorArchitecture" );
        public static readonly XName BindingRedirect = XName.Get( "bindingRedirect", Namespace );
        public static readonly XName OldVersion = XName.Get( "oldVersion" );
        public static readonly XName NewVersion = XName.Get( "newVersion" );
        public static readonly XName PublisherPolicy = XName.Get( "publisherPolicy" );
        public static readonly XName Apply = XName.Get( "apply" );
        public static readonly XName CodeBase = XName.Get( "codeBase" );
        public static readonly XName HRef = XName.Get( "href" );
        public static readonly XName Version = XName.Get( "version" );

    }
}
