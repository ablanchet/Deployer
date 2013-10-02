using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CK.Core;
using Deployer.Settings;
using Deployer.Settings.Validity;

namespace Deployer.Action
{
    public interface IAction
    {
        IEnumerable<string> PatternMatchers { get; }

        string Description { get; }

        ISettings LoadSettings( ISettingsLoader loader, ISettingsValidityCollector collector, IList<string> extraParameters, IActivityLogger logger );

        void CheckSettingsValidity( ISettings settings, ISettingsValidityCollector collector, IList<string> extraParameters, IActivityLogger logger );
        
        IActionResult Run( Runner runner, ISettings settings, IList<string> extraParameters, IActivityLogger logger );
    }
}
