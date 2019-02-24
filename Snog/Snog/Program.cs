using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Snog
{
    class Program
    {
        static void Main(string[] args)
        {
            string program = @"
            ESKETIT

            SÆT HELTAL $a = 10
            SÆT HELTAL $b = 15

            SÆT DECIMALTAL $c = 7.5

            UDSKRIV $a
            UDSKRIV $b
            UDSKRIV $c

            SKRRT SKRRT";

            RunProgram(program);
        }

        private static void RunProgram(string program)
        {
            var lexer = SnogLexer.LoadLexer(program);
            var tokens = lexer.GetTokens();
            Console.WriteLine("Vi har nu følgende tokens:");
            foreach (var token in tokens)
            {
                Console.WriteLine(token);
            }
        }
    }

    class SnogLexer
    {
        private string[] latestRegexMatch;
        public string Program { get; set; }
        private SnogLexer(string program)
        {
            Program = program;
        }

        public static SnogLexer LoadLexer(string program)
        {
            return new SnogLexer(program);
        }

        public IEnumerable<Token> GetTokens()
        {
            return Program.Split('\n')
                .Select(e => e.Trim())
                .Where(e => !string.IsNullOrWhiteSpace(e))
                .Select(GetTokenFromLine)
                .ToList();
        }

        private Token GetTokenFromLine(string line)
        {
            if (line == "ESKETIT") return new Token(Token.Symbols.START);
            if (line == "SKRRT SKRRT") return new Token(Token.Symbols.END);

            if (LineMatches(line, "^SÆT\\s+HELTAL\\s+\\$([a-zA_Z][a-zA-Z_0-9]*)\\s*=\\s*([1-9][0-9]*)$"))
            {
                return new Token(Token.Symbols.INTEGER_DECLARATION, latestRegexMatch[1], latestRegexMatch[2]);
            }

            if (LineMatches(line, "^SÆT\\s+DECIMALTAL\\s+\\$([a-zA_Z][a-zA-Z_0-9]*)\\s*=\\s*([1-9][0-9]*(\\.[0-9]+)?)$"))
            {
                return new Token(Token.Symbols.FLOAT_DECLARATION, latestRegexMatch[1], latestRegexMatch[2]);
            }

            if (LineMatches(line, "^UDSKRIV\\s+\\$([a-zA_Z][a-zA-Z_0-9]*)$"))
            {
                return new Token(Token.Symbols.PRINT, latestRegexMatch[1]);
            }

            throw new Exception("Lexical error on: " + line);
        }

        private bool LineMatches(string line, string pattern)
        {
            var match = Regex.Match(line, pattern);
            if (match.Success)
            {
                latestRegexMatch = match.Groups.Select(e => e.Value).ToArray();
                return true;
            }
            latestRegexMatch = null;
            return false;
        }
    }

    class Token
    {
        public enum Symbols
        {
            START, END, ID, PRINT, INTEGER_DECLARATION, FLOAT_DECLARATION
        }

        public Symbols Symbol { get; set; }
        public string Value { get; set; }
        public string Value2 { get; set; }

        public Token(Symbols symbol): this(symbol, null)
        {
        }

        public Token(Symbols symbol, string value) : this(symbol, value, null)
        {
        }

        public Token(Symbols symbol, string value, string value2)
        {
            Symbol = symbol;
            Value = value;
            Value2 = value2;
        }

        public override string ToString()
        {
            return $"{Symbol} {Value} {Value2}";
        }
    }
}
