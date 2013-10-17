using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deployer.Utils
{
    public static class DateHelper
    {
        public static string ToFileFormatString( this DateTime @this )
        {
            return @this.ToString( "yyyy-MM-dd HH-mm ss" );
        }
    }

}
