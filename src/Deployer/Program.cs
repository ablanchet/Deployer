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
            logger.Tap.Register( new ActivityLoggerConsoleSink() );

            Runner runner = new Runner( logger );
            DiscoverAndRegisterActions( runner );

            var result = runner.Run( args );

            if( Console.CursorLeft > 0 )
                Console.WriteLine();

            foreach( var res in result.Results )
            {
                if( res.Level == Deployer.Results.ResultLevel.Success )
                    using( ConsoleHelper.ScopeForegroundColor( ConsoleColor.Green ) )
                        Console.WriteLine( "(success) " + res.Message );
                else if( res.Level == Deployer.Results.ResultLevel.Info )
                    using( ConsoleHelper.ScopeForegroundColor( ConsoleColor.Cyan ) )
                        Console.WriteLine( "(info) " + res.Message );
                else if( res.Level == Deployer.Results.ResultLevel.Warning )
                    using( ConsoleHelper.ScopeForegroundColor( ConsoleColor.Yellow ) )
                        Console.WriteLine( "(warn) " + res.Message );
                else if( res.Level == Deployer.Results.ResultLevel.Error )
                    using( ConsoleHelper.ScopeForegroundColor( ConsoleColor.Red ) )
                        Console.WriteLine( "(err) " + res.Message );
            }
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
