using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Deployer.Settings.Impl
{
    public class XmlSettings : ISettings
    {
        List<string> _dllPaths;
        List<string> _assembliesToProcess;
        ReadOnlyCollection<string> _roDllPaths;
        ReadOnlyCollection<string> _roAssembliesToProcess;

        public XmlSettings()
        {
            _dllPaths = new List<string>();
            _assembliesToProcess = new List<string>();

            _roDllPaths = new ReadOnlyCollection<string>( _dllPaths );
            _roAssembliesToProcess = new ReadOnlyCollection<string>( _assembliesToProcess );
        }

        [XmlIgnore]
        public bool IsNew { get { return string.IsNullOrEmpty( FilePath ); } }

        [XmlIgnore]
        public string FilePath { get; set; }

        public string RootAbsoluteDirectory { get; set; }

        public string BackupDirectory { get; set; }

        public string LogDirectory { get; set; }

        public string[] SerializedDllPaths
        {
            get { return _dllPaths.ToArray(); }
            set { _dllPaths.Clear(); _dllPaths.InsertRange( 0, value ); }
        }

        public string[] SerializedAssembliesToProcess
        {
            get { return _assembliesToProcess.ToArray(); }
            set { _assembliesToProcess.Clear(); _assembliesToProcess.InsertRange( 0, value ); }
        }
        
        public string DBSetupConsolePath { get; set; }

        public string ConnectionString { get; set; }

        IReadOnlyCollection<string> ISettings.DllDirectoryPaths
        {
            get { return _roDllPaths; }
        }

        IReadOnlyCollection<string> ISettings.AssemblieNamesToProcess
        {
            get { return _roAssembliesToProcess; }
        }

        internal void UpdateWith( ISettings newOnes )
        {
            FilePath = newOnes.FilePath;
            RootAbsoluteDirectory = newOnes.RootAbsoluteDirectory;
            BackupDirectory = newOnes.BackupDirectory;
            LogDirectory = newOnes.LogDirectory;
            _dllPaths.Clear(); _dllPaths.InsertRange( 0, newOnes.DllDirectoryPaths );
            _assembliesToProcess.Clear(); _assembliesToProcess.InsertRange( 0, newOnes.AssemblieNamesToProcess );
            ConnectionString = newOnes.ConnectionString;
            DBSetupConsolePath = newOnes.DBSetupConsolePath;
        }
    }
}
