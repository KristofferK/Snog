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

            SÆT DECIMALTAL $malou_alder = 5.8
            SÆT DECIMALTAL $malou_vægt = 7.5
            SÆT HELTAL $malou_udseende = 10

            REDIGER $malou_alder = 5

            UDSKRIV $malou_alder
            UDSKRIV $malou_vægt
            UDSKRIV $malou_udseende

            REDIGER $malou_udseende = 9.5

            UDSKRIV $malou_udseende

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

            Console.WriteLine("\n\nStarter interpreter");

            var interpreter = new SnogInterpreter();
            interpreter.Interpret(tokens);

            Console.WriteLine("\n\nPython");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(interpreter.PythonString);
        }
    }

    class SymbolEntry
    {
        public string Value { get; set; }
        public Type Type { get; set; }
    }

    class SnogInterpreter
    {
        private Dictionary<string, SymbolEntry> symboltable;
        private bool hasStarted;
        private bool hasEnded;
        private string errorMessage;

        public string PythonString { get; private set; }

        public void Interpret(IEnumerable<Token> tokens)
        {
            Reset();
            foreach (var token in tokens)
            {
                if (token.Symbol == Token.Symbols.START)
                {
                    if (hasStarted)
                    {
                        errorMessage = "Der må ikke være flere start tokens";
                        break;
                    }
                    hasStarted = true;
                    continue;
                }
                if (!hasStarted)
                {
                    errorMessage = "Mangler start token";
                    break;
                }

                if (token.Symbol == Token.Symbols.END)
                {
                    break;
                }

                else if (token.Symbol == Token.Symbols.INTEGER_SET)
                {
                    symboltable[token.Value] = new SymbolEntry() { Value = token.Value2, Type = typeof(int) };
                    PythonString += $"{token.Value} = {token.Value2}\n";
                }

                else if (token.Symbol == Token.Symbols.FLOAT_SET)
                {
                    symboltable[token.Value] = new SymbolEntry() { Value = token.Value2, Type = typeof(float) };
                    PythonString += $"{token.Value} = {token.Value2}\n";
                }

                else if (token.Symbol == Token.Symbols.FLOAT_EDIT)
                {
                    if (!symboltable.ContainsKey(token.Value))
                    {
                        errorMessage = "Kan ikke redigere den udefinerede variabel $" + token.Value;
                        break;
                    }
                    if (symboltable[token.Value].Type == typeof(int))
                    {
                        errorMessage = "Kan ikke redigere variabel $" + token.Value + " af typen integer til en float værdi";
                        break;
                    }
                    symboltable[token.Value] = new SymbolEntry() { Value = token.Value2, Type = typeof(float) };
                    PythonString += $"{token.Value} = {token.Value2}\n";
                }

                else if (token.Symbol == Token.Symbols.INTEGER_EDIT)
                {
                    if (!symboltable.ContainsKey(token.Value))
                    {
                        errorMessage = "Kan ikke redigere den udefinerede variabel $" + token.Value;
                        break;
                    }
                    // Opdater uden at ændre datatypen, da floats godt må indeholde ints
                    symboltable[token.Value].Value = token.Value2;
                    PythonString += $"{token.Value} = {token.Value2}\n";
                }

                else if (token.Symbol == Token.Symbols.PRINT)
                {
                    if (!symboltable.ContainsKey(token.Value))
                    {
                        errorMessage = "Kan ikke udskrive den udefinerede variabel $" + token.Value;
                        break;
                    }
                    Console.WriteLine(symboltable[token.Value].Value);
                    PythonString += $"print({token.Value})\n";
                }

                else
                {
                    errorMessage = "Interpreteren forstod ikke følgende token: " + token;
                    break;
                }
            }

            if (errorMessage != null)
            {
                var tempColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Der opstod en fejl:");
                Console.WriteLine(errorMessage + "\n\n\n");
                Console.ForegroundColor = tempColor;
            }
            else if (!hasEnded)
            {
                var tempColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Du glemte en END token.");
                Console.WriteLine("Men okay. Jeg er en interpreter, så det gør ikke så meget");
                Console.ForegroundColor = tempColor;
            }
        }

        private void Reset()
        {
            symboltable = new Dictionary<string, SymbolEntry>();
            hasStarted = false;
            hasEnded = true;
            errorMessage = null;
            PythonString = "";
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

            if (LineMatches(line, "^SÆT\\s+HELTAL\\s+\\$([a-zæøå][a-zæøå_0-9]*)\\s*=\\s*([1-9][0-9]*)$"))
            {
                return new Token(Token.Symbols.INTEGER_SET, latestRegexMatch[1], latestRegexMatch[2]);
            }

            if (LineMatches(line, "^SÆT\\s+DECIMALTAL\\s+\\$([a-zæøå][a-zæøå_0-9]*)\\s*=\\s*([1-9][0-9]*(\\.[0-9]+)?)$"))
            {
                return new Token(Token.Symbols.FLOAT_SET, latestRegexMatch[1], latestRegexMatch[2]);
            }

            if (LineMatches(line, "^REDIGER\\s+\\$([a-zæøå][a-zæøå_0-9]*)\\s*=\\s*([1-9][0-9]*)$"))
            {
                return new Token(Token.Symbols.INTEGER_EDIT, latestRegexMatch[1], latestRegexMatch[2]);
            }

            if (LineMatches(line, "^REDIGER\\s+\\$([a-zæøå][a-zæøå_0-9]*)\\s*=\\s*([1-9][0-9]*(\\.[0-9]+)?)$"))
            {
                return new Token(Token.Symbols.FLOAT_EDIT, latestRegexMatch[1], latestRegexMatch[2]);
            }

            if (LineMatches(line, "^UDSKRIV\\s+\\$([a-zæøå][a-zæøå_0-9]*)$"))
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
            START, END, PRINT, INTEGER_SET, FLOAT_SET, INTEGER_EDIT, FLOAT_EDIT
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
