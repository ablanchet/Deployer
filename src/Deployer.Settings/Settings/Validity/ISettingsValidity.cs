using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Deployer.Results;
using Deployer.Settings.Validity;

namespace Deployer.Settings.Validity
{
    public interface ISettingsValidity
    {
        bool IsValid { get; }

        IReadOnlyCollection<Result> Results { get; }
    }

    public interface ISettingsValidityCollector : ISettingsValidity
    {
        void Add( Result result );
    }
}
