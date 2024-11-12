using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using emiT_C.Legacy;

namespace emiT_C.Legacy
{
    public class Lexer
    {

        //this is very badly structured. im NOT going to fix it
        public List<Token> Tokenize(string src)
        {
            List<Token> tokens = new List<Token>();

            List<string> words = SplitWords(src);


            foreach (string str in words)
            {
                if (str.Length == 0)
                {
                    continue;
                }

                if (str[0] == '"')
                {
                    if (str[str.Length - 1] != '"')
                    {
                        throw new Exception("Unclosed String: " + str);
                    }
                    tokens.Add(new Token(str.Substring(1, str.Length - 2), TokenType.String));
                    continue;
                }
                if (int.TryParse(str, out int strInt))
                {
                    tokens.Add(new Token(str, TokenType.Int));
                    continue;
                }
                if (float.TryParse(str, out float strFloat))
                {
                    tokens.Add(new Token(str, TokenType.Float));
                    continue;
                }
                if (str.All(char.IsLetterOrDigit))
                {
                    switch (str)
                    {
                        case "create":
                            tokens.Add(new Token(str, TokenType.Create));
                            continue;
                        case "kills":
                            tokens.Add(new Token(str, TokenType.Kills));
                            continue;
                        case "dead":
                            tokens.Add(new Token(str, TokenType.Dead));
                            continue;
                        case "alive":
                            tokens.Add(new Token(str, TokenType.Alive));
                            continue;
                        case "exists":
                            tokens.Add(new Token(str, TokenType.Exists));
                            continue;
                        case "warps":
                            tokens.Add(new Token(str, TokenType.Warps));
                            continue;
                        case "waits":
                            tokens.Add(new Token(str, TokenType.Waits));
                            continue;
                        case "time":
                            tokens.Add(new Token(str, TokenType.Time));
                            continue;
                        case "collapse":
                            tokens.Add(new Token(str, TokenType.Collapse));
                            continue;

                        case "is":
                            tokens.Add(new Token(str, TokenType.Is));
                            continue;
                        case "if":
                            tokens.Add(new Token(str, TokenType.If));
                            continue;

                        case "print":
                            tokens.Add(new Token(str, TokenType.Print));
                            continue;

                        case "true":
                        case "false":
                            tokens.Add(new Token(str, TokenType.Bool));
                            continue;

                        case "int":
                        case "float":
                        case "string":
                            tokens.Add(new Token(str, TokenType.Type));
                            continue;
                    }
                    tokens.Add(new Token(str, TokenType.Symbol));
                    continue;
                }

                GetSymbols(str);
            }

            tokens.Add(new Token("EOF", TokenType.EOF));

            return tokens;



            void GetSymbols(string str)
            {
                int substringStart = 0;
                int length = str.Length + 1;
                while (str.Length > substringStart)
                {
                    length--;
                    string str1 = str.Substring(substringStart, length);
                    if (CheckSymbols(str1))
                    {
                        substringStart += length;
                        length = str.Length + 1 - substringStart;
                    }

                    if (length < 1)
                    {
                        throw new Exception("Unrecognised symbol: " + str);
                    }
                }


            }

            bool CheckSymbols(string str)
            {
                switch (str)
                {


                    case "==":
                    case ">":
                    case "<":
                        tokens.Add(new Token(str, TokenType.BooleanOp));
                        return true;

                    case "+":
                    case "-":
                    case "*":
                    case "/":
                        tokens.Add(new Token(str, TokenType.BinaryOp));
                        return true;

                    case "=":
                        tokens.Add(new Token(str, TokenType.Assign));
                        return true;

                    case "{":
                        tokens.Add(new Token(str, TokenType.OpenBracket));
                        return true;
                    case "}":
                        tokens.Add(new Token(str, TokenType.CloseBracket));
                        return true;
                    case "(":
                        tokens.Add(new Token(str, TokenType.OpenParen));
                        return true;
                    case ")":
                        tokens.Add(new Token(str, TokenType.CloseParen));
                        return true;
                    case ";":
                        tokens.Add(new Token(str, TokenType.SemiColon));
                        return true;

                    default:
                        return false;

                }
            }
        }


        List<string> SplitWords(string src)
        {
            string[] lines = src.Split('\n', '\r'); //split source string into lines

            for (int i = 0; i < lines.Length; i++)
            {
                lines[i] = lines[i].Split("//")[0]; //trim comments out of the lines
            }

            List<string> words = new List<string>();

            foreach (string line in lines) // i am aware that this is unreadable garbage. however i do not care.
            {
                string[] str1 = line.Split('\n', ' ', '\r');

                foreach (string str2 in str1)
                {
                    List<string> words1 = new List<string> { string.Empty };
                    for (var i = 0; i < str2.Length; i++)
                    {
                        words1[words1.Count - 1] += str2[i];
                        if (i + 1 < str2.Length && ((str2[i] == '\"' || char.IsLetter(str2[i])) != (str2[i + 1] == '\"' || char.IsLetter(str2[i + 1])) || char.IsDigit(str2[i]) != char.IsDigit(str2[i + 1])))
                        {
                            words1.Add(string.Empty);
                        }
                    }

                    words.AddRange(words1);
                }
            }

            return words;
        }
    }
}
