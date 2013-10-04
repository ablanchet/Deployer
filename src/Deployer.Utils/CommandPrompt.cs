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
            PrintQuestion( question, defaultAnswer, null );
            string answer = Console.ReadLine();
            answer = string.IsNullOrEmpty( answer ) ? defaultAnswer : answer;

            if( isValid == null || isValid( answer ) )
                return answer;

            PrintWrongAnswer();
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
                PrintWrongAnswer();
                return PromptInt( question, defaultAnswer, isValid );
            }

            if( isValid == null || isValid( answer ) )
                return answer;

            PrintWrongAnswer();
            return PromptInt( question, defaultAnswer, isValid );
        }

        public static bool PromptBool( string question, string defaultAnswer = null )
        {
            PrintQuestion( question + " (y/n)", defaultAnswer, null, false );
            string rawAnswer =  Console.ReadLine();

            rawAnswer = string.IsNullOrEmpty( rawAnswer ) && !string.IsNullOrEmpty( defaultAnswer ) ? defaultAnswer : rawAnswer;

            if( rawAnswer == "y" || rawAnswer == "yes" ) return true;
            else if( rawAnswer == "n" || rawAnswer == "no" ) return false;

            PrintWrongAnswer();
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
                        PrintWrongAnswer();
                    else
                        answers.Add( answer );
                }
                else break;
            }

            if( answers.Count == 0 )
            {
                if( defaultAnwser == null || defaultAnwser.Length == 0 )
                {
                    PrintWrongAnswer();
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
                Console.Write( " (current: \"{0}\")", defaultValue );
            }
            Console.Write( ": " );

            if( breakline )
                Console.Write( Environment.NewLine );
        }

        static void PrintWrongAnswer()
        {
            using( ConsoleHelper.ScopeForegroundColor( ConsoleColor.Red ) )
            {
                Console.WriteLine( "Unrecognized answer" );
            }
        }
    }
}
