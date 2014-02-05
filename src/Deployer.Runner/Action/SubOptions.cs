using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deployer.Action
{
    /// <summary>
    /// SubOptions are used to display help correctly for sub arguments of actions
    /// </summary>
    public class SubOptions
    {
        /// <summary>
        /// ArgumentName will be display after action otpion like :
        /// -dbsetup --from=       Description texte
        /// Here the argument name is "--from="
        /// </summary>
        public string ArgumentName { get; set; }

        /// <summary>
        /// Description text for the action with this argument
        /// </summary>
        public string Description { get; set; }
    }
}
