using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace emiT_C
{
    public class ImprovedLexer
    {
        private string src;

        private int start = 0;
        private int length = 0;
        private int current => start+length;
        private int line = 1;

        List<Token> tokens;

        private bool IsAtEnd => current >= src.Length;

        static Dictionary<string, TokenType> keywords = new Dictionary<string, TokenType>
        {
            {"if", TokenType.If },
            {"is", TokenType.Is },
            {"print", TokenType.Print },
            {"create", TokenType.Create },
            {"kills", TokenType.Kills },
            {"warps", TokenType.Warps },
            {"time", TokenType.Time },
            {"dead", TokenType.Dead },
            {"alive", TokenType.Alive },
            {"exists", TokenType.Exists },
            {"collapse", TokenType.Collapse },

        };

        public ImprovedLexer(string src)
        {
            this.src = src;
        }

        public List<Token> Tokenize()
        {
            tokens = new List<Token>();
            start = 0; //reassert that these are the correct starting values
            length = 0;
            line = 1;

            while (!IsAtEnd)
            {
                start += length;
                length = 0;
                ScanToken();
            }

            tokens.Add(new Token("EOF",TokenType.EOF, null,line));

            return tokens;
        }

        void ScanToken()
        {
            char c = Eat();
            switch (c)
            {
                case '(': AddToken(TokenType.OpenParen); break;
                case ')': AddToken(TokenType.CloseParen); break;
                case '{': AddToken(TokenType.OpenBracket); break;
                case '}': AddToken(TokenType.CloseBracket); break;

                case '-':
                    AddToken(TokenType.Subtract); break;
                case '+':
                    AddToken(TokenType.Add); break;
                case '*':
                    AddToken(TokenType.Multiply); break;
                case '%':
                    AddToken(TokenType.Modulus); break;



                case ';': AddToken(TokenType.SemiColon); break;

                case '!':
                    AddToken(Match('=') ? TokenType.NotEquals : TokenType.Not); // != | !
                    break;
                case '=':
                    AddToken(Match('=') ? TokenType.Equals : TokenType.Assign); // == | =
                    break;
                case '>':
                    AddToken(Match('=') ? TokenType.GreaterOrEqual : TokenType.Greater); // <= | <
                    break;
                case '<':
                    AddToken(Match('=') ? TokenType.LessOrEqual : TokenType.Less);  // <= | <
                    break;
                case '/':
                    if (Match('/'))
                    {
                        // A comment goes until the end of the line.
                        while (Peek() != '\n' && !IsAtEnd)
                        {
                            Eat();
                        }
                    }
                    else
                    {
                        AddToken(TokenType.Divide);
                    }
                    break;


                case ' ':
                case '\r':
                case '\t':
                    break;

                case '\n':
                    line++;
                    break;


                case '"':
                    GetString(); break;



                default:
                    if (char.IsDigit(c))
                    {
                        GetNumber(); break;
                    }
                    if (char.IsLetter(c))
                    {
                        GetIdentifier(); break;
                    }

                    throw new Exception("Unexpected character at line " + line);
            }
        }

        void GetIdentifier()
        {
            while (char.IsLetterOrDigit(Peek()))
            {
                Eat();
            }
            if (keywords.TryGetValue(src.Substring(start, length), out TokenType value))
            {
                AddToken(value);
                return;
            }

            AddToken(TokenType.Symbol);
        }

        void GetNumber()
        {
            while (char.IsDigit(Peek()))
            {
                Eat();
            }



            if(Peek() == '.' && char.IsDigit(PeekNext())) // has fractional part
            {
                Eat();

                while (char.IsDigit(Peek()))
                {
                    Eat();
                }

                AddToken(TokenType.Float,float.Parse(src.Substring(start,length)));
                return;
            }

            if(Peek() == 'f') //allows defining floats that dont have a decimal part
            {
                Eat();
                AddToken(TokenType.Float, float.Parse(src.Substring(start, length-1)));
                return;
            }

            AddToken(TokenType.Int, int.Parse(src.Substring(start, length)));
        }

        void GetString()
        {
            while (Peek() != '"' && !IsAtEnd)
            {
                if (Peek() == '\n') line++; //multi line strings
                Eat();
            }

            if (IsAtEnd)
            {
                throw new Exception("Unclosed string at line " + line);
            }

            Eat(); //the closing "

            string value = src.Substring(start + 1, length - 1);
            AddToken(TokenType.String, value);
        }

        bool Match(char expect)
        {
            if (IsAtEnd) return false;
            if (src[start + length] != expect) return false;

            length++;
            return true;
        }

        char Eat()
        {
            return src[start + length++];
        }

        private char Peek()
        {
            if (IsAtEnd) return '\0';
            return src[start + length];
        }

        private char PeekNext()
        {
            if (length + 1 >= src.Length) return '\0';
            return src[start + length + 1];
        }

        void AddToken(TokenType type)
        {
            AddToken(type, null);
        }
        void AddToken(TokenType type, object value)
        {
            tokens.Add(new Token(src.Substring(start, length), type, value, line));
        }
    }
}
