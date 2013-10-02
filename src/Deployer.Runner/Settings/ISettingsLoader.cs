using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deployer.Settings
{
    public interface ISettingsLoader
    {
        ISettings Load( string filepath );

        void Save( ISettings settings );
    }
}
