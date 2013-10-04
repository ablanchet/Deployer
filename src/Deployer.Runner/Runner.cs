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
using Deployer.Utils;

namespace Deployer
{
    public class Runner
    {
        IActivityLogger _logger;

        ISettingsLoader _settingsLoader;
        IDictionary<Type,ActionWrapper> _actions;
        OptionSet _optionSet;
        ShowHelpAction _helpAction;

        public Runner( IActivityLogger logger )
        {
            _logger = logger;

            _settingsLoader = new XmlSettingsLoader();
            _actions = new Dictionary<Type, ActionWrapper>();
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

        public void RegisterAction( IAction action )
        {
            ActionWrapper wrapper = new ActionWrapper( action );
            _actions.Add( action.GetType(), wrapper );

            _optionSet.Add( wrapper.Prototype, wrapper.UnderlyingAction.Description, v => wrapper.ShouldRun = v != null );
        }

        public void Run( string[] arguments )
        {
            if( arguments == null || arguments.Length == 0 )
            {
                _helpAction.Run( this, null, null, _logger );
                return;
            }

            List<string> extraParameters = new List<string>();
            try
            {
                extraParameters = _optionSet.Parse( arguments );
            }
            catch( Exception ex )
            {
                _logger.Error( ex );
                _helpAction.Run( this, null, null, _logger );
                return;
            }
            ActionWrapper actionToRun = _actions.Values.FirstOrDefault( a => a.ShouldRun );
            if( actionToRun != null )
            {
                ISettings settings = null;

                int innerErrorCount = 0;
                using( _logger.CatchCounter( ( errorCount ) => innerErrorCount = errorCount ) )
                    settings = actionToRun.UnderlyingAction.LoadSettings( _settingsLoader, extraParameters, _logger );

                if( settings != null && innerErrorCount == 0 )
                    RunAction( actionToRun.UnderlyingAction, settings, extraParameters );
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

        public void RunSpecificAction<T>( ISettings settings, IList<string> extraParameters )
            where T : IAction
        {
            ActionWrapper actionToRun = null;
            if( _actions.TryGetValue( typeof( T ), out actionToRun ) )
            {
                RunAction( actionToRun.UnderlyingAction, settings, extraParameters );
            }
            else
                _logger.Error( "The action with type {0} cannot be found", typeof( T ) );
        }

        void RunAction( IAction action, ISettings settings, IList<string> extraParameters )
        {
            int innerErrorCount = 0;
            using( _logger.CatchCounter( ( errorCount ) => innerErrorCount = errorCount ) )
                action.CheckSettingsValidity( settings, extraParameters, _logger );

            if( innerErrorCount == 0 )
                action.Run( this, settings, extraParameters, _logger );
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
                return loader.Load( null, logger );
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
