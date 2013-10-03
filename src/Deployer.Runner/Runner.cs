using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Deployer.Settings;
using Deployer.Settings.Impl;
using Deployer.Action;
using Mono.Options;
using CK.Core;

namespace Deployer
{
    public class Runner
    {
        IActivityLogger _logger;

        ISettingsLoader _settingsLoader;
        List<ActionWrapper> _actions;
        OptionSet _optionSet;
        ShowHelpAction _helpAction;

        int _errorCount;

        public Runner( IActivityLogger logger )
        {
            _logger = logger;

            _settingsLoader = new XmlSettingsLoader();
            _actions = new List<ActionWrapper>();
            _optionSet = new OptionSet();

            _helpAction = new ShowHelpAction( this );
            RegisterAction( _helpAction );
        }

        class ActionWrapper
        {
            IAction _action;

            public ActionWrapper( IAction action )
            {
                _action = action;
            }

            public IAction UnderlyingAction { get { return _action; } }

            public string Prototype { get { return string.Join( "|", _action.PatternMatchers ); } }

            public bool ShouldRun { get; set; }
        }

        public void RegisterAction( IAction action )
        {
            ActionWrapper wrapper = new ActionWrapper( action );
            _actions.Add( wrapper );

            _optionSet.Add( wrapper.Prototype, wrapper.UnderlyingAction.Description, v => wrapper.ShouldRun = v != null );
        }

        public void Run( string[] arguments )
        {
            if( arguments == null || arguments.Length == 0 )
            {
                _helpAction.Run( this, null, null, _logger );
                return;
            }

            List<string> extraParameters = _optionSet.Parse( arguments );

            ActionWrapper actionToRun = _actions.FirstOrDefault( a => a.ShouldRun );
            if( actionToRun != null )
            {
                ISettings settings = null;
                using( _logger.CatchCounter( ( errorCount ) => _errorCount = errorCount ) )
                    settings = actionToRun.UnderlyingAction.LoadSettings( _settingsLoader, extraParameters, _logger );

                if( settings != null && _errorCount == 0 )
                {
                    using( _logger.CatchCounter( ( errorCount ) => _errorCount = errorCount ) )
                        actionToRun.UnderlyingAction.CheckSettingsValidity( settings, extraParameters, _logger );

                    if( _errorCount == 0 )
                        actionToRun.UnderlyingAction.Run( this, settings, extraParameters, _logger );
                }
            }
            else
            {
                _logger.Error( "The command is invalid. Try -help to show usage" );
            }
        }

        public void UpdateSettings( ISettings source, ISettings newOnes )
        {
            XmlSettings xmlSource = (XmlSettings)source;
            xmlSource.UpdateWith( newOnes );

            _settingsLoader.Save( xmlSource );
        }


        class ShowHelpAction : IAction
        {
            Runner _commandRunner;
            static string[] _patterns = new string[] { "h", "?", "help" };

            public ShowHelpAction( Runner commandRunner )
            {
                _commandRunner = commandRunner;
            }

            public IEnumerable<string> PatternMatchers
            {
                get { return _patterns; }
            }

            public string Description
            {
                get { return "Show the program usage. All commands available, and their descriptions"; }
            }

            public void CheckSettingsValidity( ISettings settings, IList<string> extraParameters, IActivityLogger logger )
            {
            }

            public ISettings LoadSettings( ISettingsLoader loader, IList<string> extraParameters, IActivityLogger logger )
            {
                return loader.Load( null );
            }

            public void Run( Runner runner, ISettings settings, IList<string> extraParameters, IActivityLogger logger )
            {
                string version = typeof( ShowHelpAction ).Assembly.GetName().Version.ToString( 4 );

                Console.WriteLine( "Deployer (version {0}){1}  Usage informations : ", version, Environment.NewLine );
                _commandRunner._optionSet.WriteOptionDescriptions( Console.Out );
            }
        }
    }
}
