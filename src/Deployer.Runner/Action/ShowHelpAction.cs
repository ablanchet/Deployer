using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CK.Core;
using Deployer.Settings;
using Mono.Options;

namespace Deployer.Action
{
    internal class ShowHelpAction : IAction
    {
        OptionSet _optionSet;
        static string[] _patterns = new string[] { "h", "?", "help" };

        public ShowHelpAction( OptionSet optionSet )
        {
            _optionSet = optionSet;
        }

        public IEnumerable<string> PatternMatchers
        {
            get { return _patterns; }
        }

        public string Description
        {
            get { return "Show the program usage. All commands available, and their descriptions"; }
        }

        public void CheckSettingsValidity( ISettings settings, IList<string> extraParameters, IActivityLogger logger )
        {
        }

        public ISettings LoadSettings( ISettingsLoader loader, IList<string> extraParameters, IActivityLogger logger )
        {
            return null;
        }

        public void Run( Runner runner, ISettings settings, IList<string> extraParameters, IActivityLogger logger )
        {
            string version = typeof( ShowHelpAction ).Assembly.GetName().Version.ToString( 4 );

            Console.WriteLine( "{1}Deployer (version {0}){1}{1}  Usage informations : {1}", version, Environment.NewLine );
            _optionSet.WriteOptionDescriptions( Console.Out );
        }

        public IEnumerable<SubOptions> GetSubOptions()
        {
            return null;
        }
    }
}
