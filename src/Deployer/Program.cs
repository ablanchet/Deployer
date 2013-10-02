using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CK.Core;
using Deployer.Action;
using Deployer.Actions;
using Deployer.Utils;

namespace Deployer
{
    class Program
    {
        static void Main( string[] args )
        {
            DefaultActivityLogger logger = new DefaultActivityLogger();
            logger.Tap.Register( new ColoredActivityLoggerConsoleSink() );

            Runner runner = new Runner( logger );
            DiscoverAndRegisterActions( runner );

            runner.Run( args );
        }

        static void DiscoverAndRegisterActions( Runner runner )
        {
            Assembly assemblyToProcess = typeof( SettingsConfigurator ).Assembly;

            foreach( var type in assemblyToProcess.GetTypes() )
            {
                if( typeof( IAction ).IsAssignableFrom( type ) )
                {
                    runner.RegisterAction( (IAction)Activator.CreateInstance( type ) );
                }
            }
        }
    }
}
