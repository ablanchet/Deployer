using System;
using System.Xml.Linq;
using CK.Core;
using Deployer.Utils.Rebindings;

namespace Deployer.Utils
{
    public class AssemblyBindingElement : IEquatable<AssemblyBindingElement>
    {
        string _name;
        string _culture;
        string _publicKeyToken;
        string _processorArchi;
        string _newVersion;
        string _oldVersion;
        string _codeBaseHref;
        string _codeBaseVersion;
        string _publisherPolicy;

        public AssemblyBindingElement()
        {
            Reset();
        }

        /// <summary>
        /// Gets ors sets the assemblly name.
        /// When null or empty, <see cref="IsValid"/> is false.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set
            {
                if( value == null ) value = String.Empty;
                _name = value;
            }
        }

        public bool IsValid
        {
            get { return _name.Length > 0; }
        }

        /// <summary>
        /// Defaults to "neutral".
        /// </summary>
        public string Culture
        {
            get { return _culture; }
            set
            {
                if( String.IsNullOrWhiteSpace( value ) ) value = "neutral";
                _culture = value;
            }
        }

        /// <summary>
        /// Defaults to null.
        /// </summary>
        public string PublicKeyToken
        {
            get { return _publicKeyToken; }
            set
            {
                if( String.IsNullOrWhiteSpace( value ) ) value = null;
                _publicKeyToken = value;
            }
        }

        public string ProcessorArchitecture
        {
            get { return _processorArchi; }
            set
            {
                if( String.IsNullOrWhiteSpace( value ) ) value = String.Empty;
                _processorArchi = value;
            }
        }

        public string NewVersion
        {
            get { return _newVersion; }
            set
            {
                if( String.IsNullOrWhiteSpace( value ) ) value = String.Empty;
                _newVersion = value;
            }
        }

        public string OldVersion
        {
            get { return _oldVersion ?? "0.0.0.0-" + _newVersion; }
            set
            {
                if( String.IsNullOrWhiteSpace( value ) ) value = null;
                _oldVersion = value;
            }
        }

        public string CodeBaseHref
        {
            get { return _codeBaseHref; }
            set
            {
                if( String.IsNullOrWhiteSpace( value ) ) value = String.Empty;
                _codeBaseHref = value;
            }
        }

        public string CodeBaseVersion
        {
            get { return _codeBaseVersion; }
            set
            {
                if( String.IsNullOrWhiteSpace( value ) ) value = String.Empty;
                _codeBaseVersion = value;
            }
        }

        public string PublisherPolicy
        {
            get { return _publisherPolicy; }
            set
            {
                if( String.IsNullOrWhiteSpace( value ) ) value = String.Empty;
                _publisherPolicy = value;
            }
        }

        public void Reset()
        {
            _name = String.Empty;
            _culture = "neutral";
            _publicKeyToken = null;
            _processorArchi = String.Empty;
            _newVersion = String.Empty;
            _oldVersion = null;
            _codeBaseHref = String.Empty;
            _codeBaseVersion = String.Empty;
            _publisherPolicy = String.Empty;
        }

        /// <summary>
        /// Returns &lt;dependentAssembly /&gt; element.
        /// </summary>
        /// <remarks>
        /// Element is:
        /// <code>
        ///     &lt;dependentAssembly&gt; 
        ///        &lt;assemblyIdentity name="{Name}" 
        ///                          publicKeyToken="{PublicKeyToken}" 
        ///                          culture="{Culture}" 
        ///                          processorArchitecture="{ProcessorArchitecture}" /&gt;
        ///     
        ///        &lt;bindingRedirect oldVersion="{OldVersion}" 
        ///                         newVersion="{NewVersion}"/&gt;
        ///     
        ///        &lt;publisherPolicy apply="{PublisherPolicy}" /&gt;
        ///     
        ///        &lt;codeBase href="{CodeBaseHref}" version="{CodeBaseVersion}" /&gt;
        ///     &lt;/dependentAssembly&gt;
        /// </code>
        /// </remarks>
        /// <returns>The assembly binding element (see remarks).</returns>
        public XElement ToXml()
        {
            XElement a = new XElement( MicrosoftAsmV1.DependentAssembly );

            XElement i = new XElement( MicrosoftAsmV1.AssemblyIdentity );
            i.Add( new XAttribute( MicrosoftAsmV1.Name, Name ), new XAttribute( MicrosoftAsmV1.Culture, _culture ) );
            if( _publicKeyToken != null ) i.Add( new XAttribute( MicrosoftAsmV1.PublicKeyToken, _publicKeyToken ) );
            if( _processorArchi.Length > 0 ) i.Add( new XAttribute( MicrosoftAsmV1.ProcessorArchitecture, _processorArchi ) );
            a.Add( i );

            XElement b = new XElement( MicrosoftAsmV1.BindingRedirect,
                                 new XAttribute( MicrosoftAsmV1.OldVersion, OldVersion ),
                                 new XAttribute( MicrosoftAsmV1.NewVersion, NewVersion ) );
            a.Add( b );

            if( _publisherPolicy.Length > 0 )
            {
                a.Add( new XElement( MicrosoftAsmV1.PublisherPolicy, new XAttribute( MicrosoftAsmV1.Apply, PublisherPolicy ) ) );
            }
            if( _codeBaseHref.Length > 0 )
            {
                XElement c = new XElement( MicrosoftAsmV1.CodeBase, new XAttribute( MicrosoftAsmV1.HRef, _codeBaseHref ) );
                if( _codeBaseVersion.Length > 0 ) c.Add( new XAttribute( MicrosoftAsmV1.Version, _codeBaseVersion ) );
                a.Add( c );
            }

            return a;
        }

        /// <summary>
        /// Resets and then parses a &lt;dependentAssembly /&gt; element of an &lt;assemblyBinding /&gt; section in a config file.
        /// </summary>
        /// <param name="dependentAssemblyElement">The &lt;dependentAssembly /&gt; element.</param>
        /// <returns>An AssemblyBinding object.</returns>
        public void FromXml( XElement dependentAssemblyElement )
        {
            if( dependentAssemblyElement == null ) throw new ArgumentNullException( "dependentAssemblyElement" );

            Reset();

            XElement a = dependentAssemblyElement.Element( MicrosoftAsmV1.AssemblyIdentity );
            if( a != null )
            {
                Name = a.GetAttribute( MicrosoftAsmV1.Name, null );
                Culture = a.GetAttribute( MicrosoftAsmV1.Culture, null );
                PublicKeyToken = a.GetAttribute( MicrosoftAsmV1.PublicKeyToken, null );
                ProcessorArchitecture = a.GetAttribute( MicrosoftAsmV1.ProcessorArchitecture, null );
            }

            XElement bindingRedirect = dependentAssemblyElement.Element( MicrosoftAsmV1.BindingRedirect );
            if( bindingRedirect != null )
            {
                OldVersion = bindingRedirect.GetAttribute( MicrosoftAsmV1.OldVersion, null );
                NewVersion = bindingRedirect.GetAttribute( MicrosoftAsmV1.NewVersion, null );
            }

            XElement codeBase = dependentAssemblyElement.Element( MicrosoftAsmV1.CodeBase );
            if( codeBase != null )
            {
                CodeBaseHref = codeBase.GetAttribute( MicrosoftAsmV1.HRef, null );
                CodeBaseVersion = codeBase.GetAttribute( MicrosoftAsmV1.Version, null );
            }

            XElement publisherPolicy = dependentAssemblyElement.Element( MicrosoftAsmV1.PublisherPolicy );
            if( publisherPolicy != null )
            {
                PublisherPolicy = publisherPolicy.GetAttribute( MicrosoftAsmV1.Apply, null );
            }
        }

        public bool Equals( AssemblyBindingElement other )
        {
            return Name == other.Name &&
                   PublicKeyToken == other.PublicKeyToken &&
                   Culture == other.Culture &&
                   ProcessorArchitecture == other.ProcessorArchitecture;
        }

        public override bool Equals( object obj )
        {
            var other = obj as AssemblyBindingElement;
            return other != null ? Equals( other ) : false;
        }

        public override int GetHashCode()
        {
            return Util.Hash.Combine( Util.Hash.StartValue, Name, PublicKeyToken, Culture, ProcessorArchitecture ).GetHashCode();
        }

        public override string ToString()
        {
            return ToXml().ToString();
        }

    }
}
