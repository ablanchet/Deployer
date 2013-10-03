using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deployer.Utils
{
    public static class CommandLineHelper
    {
        public static string PromptString( string question, string defaultAnswer = null, Func<string, bool> isValid = null )
        {
            PrintQuestion( question, defaultAnswer, null, false );
            string answer = Console.ReadLine();
            answer = string.IsNullOrEmpty( answer ) ? defaultAnswer : answer;

            if( !string.IsNullOrEmpty( answer ) && (isValid == null || isValid( answer )) )
                return answer;

            PrintWrongAnswer( "a valid string" );
            return PromptString( question, defaultAnswer, isValid );
        }

        public static int PromptInt( string question, int? defaultAnswer = null, Func<int, bool> isValid = null )
        {
            PrintQuestion( question );
            string rawAnswer = Console.ReadLine();
            int answer = -1;
            if( !int.TryParse( rawAnswer, out answer )
                && defaultAnswer != null
                && defaultAnswer.HasValue )
            {
                answer = defaultAnswer.Value;
            }
            else
            {
                PrintWrongAnswer( "an integer" );
                return PromptInt( question, defaultAnswer, isValid );
            }

            if( isValid == null || isValid( answer ) )
                return answer;

            PrintWrongAnswer( "an integer" );
            return PromptInt( question, defaultAnswer, isValid );
        }

        public static bool PromptBool( string question, string defaultAnswer = null )
        {
            PrintQuestion( question + " (y/n)", defaultAnswer, null, false );
            string rawAnswer =  Console.ReadLine();

            rawAnswer = string.IsNullOrEmpty( rawAnswer ) && !string.IsNullOrEmpty( defaultAnswer ) ? defaultAnswer : rawAnswer;

            if( rawAnswer == "y" || rawAnswer == "yes" ) return true;
            else if( rawAnswer == "n" || rawAnswer == "no" ) return false;

            PrintWrongAnswer( "yes (y) or no (n)" );
            return PromptBool( question, defaultAnswer );
        }

        public static string[] PromptStringArray( string question, Func<string, bool> isValid = null, string[] defaultAnwser = null )
        {
            PrintQuestion( question, defaultAnwser != null ? string.Join( ", ", defaultAnwser ) : (string)null );
            List<string> answers = new List<string>();
            string answer = null;
            while( true )
            {
                answer = null;
                answer = Console.ReadLine();
                if( !string.IsNullOrEmpty( answer ) )
                {
                    if( isValid != null && !isValid( answer ) )
                        PrintWrongAnswer( "a valid string" );
                    else
                        answers.Add( answer );
                }
                else break;
            }

            if( answers.Count == 0 )
            {
                if( defaultAnwser == null || defaultAnwser.Length == 0 )
                {
                    PrintWrongAnswer( "at least a valid string" );
                    return PromptStringArray( question, isValid, defaultAnwser );
                }
                else return defaultAnwser;
            }

            return answers.ToArray();
        }

        static void PrintQuestion( string question, string defaultValue = null, object[] args = null, bool breakline = true )
        {
            Console.Write( question, args );
            if( !string.IsNullOrEmpty( defaultValue ) )
            {
                Console.Write( " (default: \"{0}\")", defaultValue );
            }
            Console.Write( ": " );

            if( breakline )
                Console.Write( Environment.NewLine );
        }

        static void PrintWrongAnswer( string expectedFormat )
        {
            using( ConsoleHelper.ScopeForegroundColor( ConsoleColor.Red ) )
            {
                Console.WriteLine( "/!\\ Unrecognized answer, please enter {0}", expectedFormat );
            }
        }
    }
}
