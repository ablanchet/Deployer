using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CK.Core;
using Deployer.Settings;

namespace Deployer.Utils
{
    public static class ConfigHelper
    {
        public static ISettings TryLoadCustomPathOrDefault( ISettingsLoader loader, IList<string> extraParameters, IActivityLogger logger )
        {
            string path = null;
            if( extraParameters.Count == 1 ) path = extraParameters[0];
            try
            {
                return loader.Load( path, logger );
            }
            catch( Exception ex )
            {
                logger.Error( ex, "Unable to load configuration" );
            }

            return null;
        }
    }
}
