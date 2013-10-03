using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CK.Core;

namespace Deployer.Settings
{
    public interface ISettingsLoader
    {
        ISettings Load( string filepath, IActivityLogger logger );

        void Save( ISettings settings );
    }
}
