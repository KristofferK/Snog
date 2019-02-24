using System;
using System.Collections.Generic;

namespace Snog
{
    class Program
    {
        static void Main(string[] args)
        {
            string program = @"
            ESKETIT

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
            var lines = Program.Split('\n');
            for (var i = 0; i < lines.Length; i++)
            {

            }
            return new List<Token>();
        }
    }

    class Token
    {
        public enum Symbols
        {
            START, END, ID, PRINT, INTEGER
        }

        public Symbols Symbol { get; set; }
        public string Value { get; set; }

        public Token(Symbols symbol): this(symbol, null)
        {
        }

        public Token(Symbols symbol, string value)
        {
            Symbol = symbol;
            Value = value;
        }
    }
}
