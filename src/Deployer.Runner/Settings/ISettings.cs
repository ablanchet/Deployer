using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deployer.Settings
{
    public interface ISettings
    {
        bool IsNew { get; }

        string FilePath { get; }

        string RootAbsoluteDirectory { get; }

        string BackupDirectory { get; }

        string LogDirectory { get; }

        IReadOnlyCollection<string> DllPaths { get; }

        IReadOnlyCollection<string> AssembliesToProcess { get; }

        string ConnectionString { get; }
    }
}
