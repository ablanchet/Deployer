using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Deployer.Results;

namespace Deployer.Action
{
    public class ActionResult : IActionResult
    {
        public ActionResult( IEnumerable<Result> results )
        {
            Results = results;
        }

        public ActionResult( ResultLevel level, string errorMessage )
        {
            Results = new Result[] { new Result( level, errorMessage ) };
        }

        public bool IsSucceeded
        {
            get { return !Results.Any( e => e.Level > ResultLevel.Warning ); }
        }

        public IEnumerable<Result> Results
        {
            get; private set;
        }
    }
}
