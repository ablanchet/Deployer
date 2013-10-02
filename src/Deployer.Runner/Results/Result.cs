using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Deployer.Results
{
    public class Result
    {
        public Result( ResultLevel level, string message )
        {
            Level = level;
            Message = message;
        }

        public ResultLevel Level { get; private set; }

        public string Message { get; private set; }

        #region Overrides

        public override int GetHashCode()
        {
            int hashCode = Level.GetHashCode();
            if( !string.IsNullOrEmpty( Message ) )
                hashCode ^= Message.GetHashCode();

            return hashCode;
        }

        public override bool Equals( object obj )
        {
            Result cObj = (Result)obj;
            return cObj.Level == this.Level && cObj.Message == this.Message;
        }

        #endregion
    }
}
