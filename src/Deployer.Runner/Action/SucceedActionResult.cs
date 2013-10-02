using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deployer.Action
{
    public class SucceedActionResult : IActionResult
    {
        private static IActionResult _result = new SucceedActionResult();

        private SucceedActionResult()
        {
        }

        public static IActionResult Result { get { return _result; } }

        public bool IsSucceeded
        {
            get { return true; }
        }

        public IEnumerable<Results.Result> Results
        {
            get { return new Results.Result[0]; }
        }
    }
}
