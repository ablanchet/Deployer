using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CK.Core;

namespace Deployer.Utils
{
    public class ColoredActivityLoggerConsoleSink : IActivityLoggerSink
    {
        Func<TextWriter> _writer;
        string _prefix;
        string _prefixLevel;
        CKTrait _currentTags;

        public ColoredActivityLoggerConsoleSink()
        {
            _writer = () => Console.Out;
            _prefixLevel = _prefix = String.Empty;
            _currentTags = ActivityLogger.EmptyTag;
        }

        void IActivityLoggerSink.OnEnterLevel( CKTrait tags, LogLevel level, string text, DateTime logTimeUtc )
        {
            using( ConsoleHelper.ScopeForegroundColor( ConvertLogLevelToConsoleColor( level ) ) )
            {
                TextWriter w = _writer();
                _prefixLevel = _prefix + new String( ' ', level.ToString().Length + 4 );
                text = text.Replace( Environment.NewLine, Environment.NewLine + _prefixLevel );
                if( _currentTags != tags )
                {
                    w.WriteLine( "{0} {1}: {2} -[{3}]", _prefix, level.ToString(), text, tags );
                    _currentTags = tags;
                }
                else
                {
                    w.WriteLine( "{0} {1}: {2}", _prefix, level.ToString(), text );
                }
            }
        }

        void IActivityLoggerSink.OnContinueOnSameLevel( CKTrait tags, LogLevel level, string text, DateTime logTimeUtc )
        {
            using( ConsoleHelper.ScopeForegroundColor( ConvertLogLevelToConsoleColor( level ) ) )
            {
                TextWriter w = _writer();
                text = text.Replace( Environment.NewLine, Environment.NewLine + _prefixLevel );
                if( _currentTags != tags )
                {
                    w.WriteLine( "{0}{1} [{2}]", _prefixLevel, text, tags );
                    _currentTags = tags;
                }
                else w.WriteLine( _prefixLevel + text );
            }
        }

        void IActivityLoggerSink.OnLeaveLevel( LogLevel level )
        {
            _prefixLevel = _prefix;
        }

        void IActivityLoggerSink.OnGroupOpen( IActivityLogGroup g )
        {
            using( ConsoleHelper.ScopeForegroundColor( ConvertLogLevelToConsoleColor( g.GroupLevel ) ) )
            {
                TextWriter w = _writer();
                string start = String.Format( "{0}> {1}: ", _prefix, g.GroupLevel.ToString() );
                _prefix += "|  ";
                _prefixLevel = _prefix;
                string text = g.GroupText.Replace( Environment.NewLine, Environment.NewLine + _prefixLevel );
                if( _currentTags != g.GroupTags )
                {
                    _currentTags = g.GroupTags;
                    w.WriteLine( "{0}{1} -[{2}]", start, text, _currentTags );
                }
                else
                {
                    w.WriteLine( start + text );
                }
            }
        }

        void IActivityLoggerSink.OnGroupClose( IActivityLogGroup g, ICKReadOnlyList<ActivityLogGroupConclusion> conclusions )
        {
            using( ConsoleHelper.ScopeForegroundColor( ConvertLogLevelToConsoleColor( g.GroupLevel ) ) )
            {
                TextWriter w = _writer();
                if( g.Exception != null )
                {
                    DumpException( w, !g.IsGroupTextTheExceptionMessage, g.Exception );
                }
                _prefixLevel = _prefix = _prefix.Remove( _prefix.Length - 3 );

                w.WriteLine( "{0}< {1}", _prefixLevel, conclusions.Where( c => !c.Text.Contains( Environment.NewLine ) ).ToStringGroupConclusion() );

                foreach( var c in conclusions.Where( c => c.Text.Contains( Environment.NewLine ) ) )
                {
                    string text = "< " + c.Text;
                    w.WriteLine( _prefixLevel + "  " + c.Text.Replace( Environment.NewLine, Environment.NewLine + _prefixLevel + "   " ) );
                }
            }
        }

        void DumpException( TextWriter w, bool displayMessage, Exception ex )
        {
            string p;
            string format = string.Format( " ┌──────────────────────────■ Exception : {0} ■──────────────────────────", ex.GetType().Name );

            w.WriteLine( _prefix + format );
            _prefix += " | ";
            string start;
            if( displayMessage && ex.Message != null )
            {
                start = _prefix + "Message: ";
                p = _prefix + "         ";
                w.WriteLine( start + ex.Message.Replace( Environment.NewLine, Environment.NewLine + p ) );
            }
            if( ex.StackTrace != null )
            {
                start = _prefix + "Stack: ";
                p = _prefix + "       ";
                w.WriteLine( start + ex.StackTrace.Replace( Environment.NewLine, Environment.NewLine + p ) );
            }
            // The InnerException of an aggregated exception is the same as the first of it InnerExceptionS.
            // (The InnerExceptionS are the contained/aggregated exceptions of the AggregatedException object.)
            // This is why, if we are on an AggregatedException we do not follow its InnerException.
            var aggrex = (ex as AggregateException);
            if( aggrex != null && aggrex.InnerExceptions.Count > 0 )
            {
                w.WriteLine( _prefix + " ┌──────────────────────────■ [Aggregated Exceptions] ■──────────────────────────" );
                _prefix += " | ";
                foreach( var item in aggrex.InnerExceptions )
                {
                    DumpException( w, true, item );
                }
                _prefix = _prefix.Remove( _prefix.Length - 3 );
                w.WriteLine( _prefix + " └─────────────────────────────────────────────────────────────────────────" );
            }
            else if( ex.InnerException != null )
            {
                w.WriteLine( _prefix + " ┌──────────────────────────■ [Inner Exception] ■──────────────────────────" );
                _prefix += " | ";
                DumpException( w, true, ex.InnerException );
                _prefix = _prefix.Remove( _prefix.Length - 3 );
                w.WriteLine( _prefix + " └─────────────────────────────────────────────────────────────────────────" );
            }
            _prefix = _prefix.Remove( _prefix.Length - 3 );
            w.WriteLine( _prefix + " └" + new string( '─', format.Length - 2 ) );
        }

        ConsoleColor ConvertLogLevelToConsoleColor( LogLevel level )
        {
            switch( level )
            {
                case LogLevel.Error:
                case LogLevel.Fatal:
                    return ConsoleColor.Red;
                case LogLevel.Info:
                    return ConsoleColor.Gray;
                case LogLevel.Warn:
                    return ConsoleColor.Yellow;
                default:
                    return ConsoleColor.White;
            }
        }
    }
}
