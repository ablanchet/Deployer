using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Deployer.Utils;

namespace Deployer.Action
{
    internal class ActionWrapper
    {
        IAction _action;

        public ActionWrapper( IAction action )
        {
            _action = action;
        }

        public IAction UnderlyingAction { get { return _action; } }

        public string Prototype
        {
            get
            {
                string proto = _action.GetType().Name;
                if( proto.ToLowerInvariant().EndsWith( "action" ) )
                    proto = proto.Substring( 0, proto.Length - 6 );

                proto = StringHelper.Humanize( proto );
                return proto;
            }
        }

        public bool ShouldRun { get; set; }
    }
}
