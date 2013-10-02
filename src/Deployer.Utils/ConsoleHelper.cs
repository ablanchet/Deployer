using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deployer.Utils
{
    public static class ConsoleHelper
    {
        class ConsoleColorRefresher : IDisposable
        {
            public void Dispose()
            {
                Console.ResetColor();
            }
        }

        public static IDisposable ScopeForegroundColor( ConsoleColor foregroundColor )
        {
            Console.ForegroundColor = foregroundColor;
            return new ConsoleColorRefresher();
        }

        public static IDisposable ScopeBackgroundColor( ConsoleColor backgroundColor )
        {
            Console.BackgroundColor = backgroundColor;
            return new ConsoleColorRefresher();
        }
    }
}
