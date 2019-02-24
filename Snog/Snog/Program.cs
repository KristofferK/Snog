using System;

namespace Snog
{
    class Program
    {
        static void Main(string[] args)
        {
            string program = @"
            ESKETIT,

            SÆT HELTAL $a = 10
            UDSKRIV $a

            SKRRT SKRRT";

            RunProgram(program);
        }

        private static void RunProgram(string program)
        {
            var lexer = SnogLexer.LoadLexer(program);
        }
    }

    class SnogLexer
    {
        public string Program { get; set; }
        private SnogLexer(string program)
        {
            Program = program;
        }

        public static SnogLexer LoadLexer(string program)
        {
            return new SnogLexer(program);
        }

        public List<Token> GetTokens()
        {
            return new List<Token>();
        }
    }

    class Token
    {
        enum Symbols
        {
            START, END, ID, PRINT, INTEGER
        }
    }
}
