using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CK.Core;
using Deployer.Settings;

namespace Deployer.Action
{
    public interface IAction
    {
        string Description { get; }

        ISettings LoadSettings( ISettingsLoader loader, IList<string> extraParameters, IActivityLogger logger );

        void CheckSettingsValidity( ISettings settings, IList<string> extraParameters, IActivityLogger logger );
        
        void Run( Runner runner, ISettings settings, IList<string> extraParameters, IActivityLogger logger );
    }
}
