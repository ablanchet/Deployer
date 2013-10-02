using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Deployer.Results;
using Deployer.Settings.Validity;

namespace Deployer.Settings.Impl
{
    internal class SettingsValidityCollector: ISettingsValidityCollector
    {
        List<Result> _errors;
        ReadOnlyCollection<Result> _roErrors;

        public SettingsValidityCollector()
        {
            _errors = new List<Result>();
            _roErrors = new ReadOnlyCollection<Result>( _errors );
        }

        public bool IsValid { get { return _errors.Count == 0; } }

        public IReadOnlyCollection<Result> Results { get { return _roErrors; } }

        public void Add( Result error )
        {
            if( !_errors.Contains( error ) )
                _errors.Add( error );
        }
    }
}
