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
            ConsoleColor _baseColor;

            public ConsoleColorRefresher( ConsoleColor baseColor )
            {
                _baseColor = baseColor;
            }

            public void Dispose()
            {
                Console.ForegroundColor = _baseColor;
            }
        }

        public static IDisposable ScopeForegroundColor( ConsoleColor foregroundColor )
        {
            ConsoleColor baseColor = Console.ForegroundColor;

            Console.ForegroundColor = foregroundColor;
            return new ConsoleColorRefresher( baseColor );
        }
    }
}
