using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Deployer.Results;

namespace Deployer.Action
{
    public interface IActionResult
    {
        bool IsSucceeded { get; }

        IEnumerable<Result> Results { get; }
    }
}
