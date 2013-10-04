using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deployer.Utils
{
    public static class StringHelper
    {
        public static string Humanize( string s )
        {
            StringBuilder sb = new StringBuilder();

            char last = char.MinValue;
            foreach( char c in s )
            {
                if( char.IsLower( last ) == true && char.IsUpper( c ) == true )
                {
                    sb.Append( '-' );
                }
                sb.Append( c );
                last = c;
            }
            return sb.ToString().ToLowerInvariant();
        }
    }

}
