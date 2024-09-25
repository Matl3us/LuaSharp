using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace LuaSharp
{
    public class Lexer(StreamReader stream, string filename)
    {
        public StreamReader Stream { get; set; } = stream;
        private readonly string _filename = filename;
        private char _char;
        private int _line = 1;
        private int _column = 1;
        private readonly Dictionary<string, TokenType> _keywords = new()
        {
            {"and", TokenType.AND}, {"break", TokenType.BREAK}, {"do", TokenType.DO}, {"else", TokenType.ELSE},
            {"elseif", TokenType.ELSEIF}, {"end", TokenType.END}, {"false", TokenType.FALSE}, {"for", TokenType.FOR},
            {"function", TokenType.FUNCTION}, {"goto", TokenType.GOTO}, {"if", TokenType.IF}, {"in", TokenType.IN},
            {"local", TokenType.LOCAL}, {"nil", TokenType.NIL}, {"not", TokenType.NOT}, {"or", TokenType.OR},
            {"repeat", TokenType.REPEAT}, {"return", TokenType.RETURN}, {"then", TokenType.THEN},
            {"true", TokenType.TRUE}, {"until", TokenType.UNTIL}, {"while", TokenType.WHILE}
        };

        public Token NextToken()
        {
            Token tok;
            if (!Stream.EndOfStream)
            {
                _char = (char)Stream.Read();

                SkipWhitespace();

                switch (_char)
                {
                    case '+':
                        tok = new Token(TokenType.PLUS, _char.ToString(), _line, _column++, _filename);
                        break;
                    case '-':
                        if ((char)Stream.Peek() == '-')
                        {
                            Stream.Read();
                            tok = new Token(TokenType.COMMENT, "", _line, _column, _filename);
                            _column += 2;
                        }
                        else
                        {
                            tok = new Token(TokenType.MINUS, _char.ToString(), _line, _column++, _filename);
                        }
                        break;
                    case '*':
                        tok = new Token(TokenType.ASTERISK, _char.ToString(), _line, _column++, _filename);
                        break;
                    case '/':
                        if ((char)Stream.Peek() == '/')
                        {
                            Stream.Read();
                            tok = new Token(TokenType.F_DIV, "//", _line, _column, _filename);
                            _column += 2;
                        }
                        else
                        {
                            tok = new Token(TokenType.SLASH, _char.ToString(), _line, _column++, _filename);
                        }
                        break;
                    case '%':
                        tok = new Token(TokenType.PERCENTAGE, _char.ToString(), _line, _column++, _filename);
                        break;
                    case '^':
                        tok = new Token(TokenType.CARET, _char.ToString(), _line, _column++, _filename);
                        break;
                    case '#':
                        tok = new Token(TokenType.HASHTAG, _char.ToString(), _line, _column++, _filename);
                        break;
                    case '<':
                        if ((char)Stream.Peek() == '=')
                        {
                            Stream.Read();
                            tok = new Token(TokenType.LESS_EQUAL, "<=", _line, _column, _filename);
                            _column += 2;
                        }
                        else if ((char)Stream.Peek() == '<')
                        {
                            Stream.Read();
                            tok = new Token(TokenType.B_LSHIFT, "<<", _line, _column, _filename);
                            _column += 2;
                        }
                        else
                        {
                            tok = new Token(TokenType.LESS, _char.ToString(), _line, _column++, _filename);
                        }
                        break;
                    case '>':
                        if ((char)Stream.Peek() == '=')
                        {
                            Stream.Read();
                            tok = new Token(TokenType.MORE_EQUAL, ">=", _line, _column, _filename);
                            _column += 2;
                        }
                        else if ((char)Stream.Peek() == '>')
                        {
                            Stream.Read();
                            tok = new Token(TokenType.B_RSHIFT, ">>", _line, _column, _filename);
                            _column += 2;
                        }
                        else
                        {
                            tok = new Token(TokenType.MORE, _char.ToString(), _line, _column++, _filename);
                        }
                        break;
                    case '=':
                        if ((char)Stream.Peek() == '=')
                        {
                            Stream.Read();
                            tok = new Token(TokenType.EQUAL, "==", _line, _column, _filename);
                            _column += 2;
                        }
                        else
                        {
                            tok = new Token(TokenType.ASSIGN, _char.ToString(), _line, _column++, _filename);
                        }
                        break;
                    case '(':
                        tok = new Token(TokenType.L_PARENT, _char.ToString(), _line, _column++, _filename);
                        break;
                    case ')':
                        tok = new Token(TokenType.R_PARENT, _char.ToString(), _line, _column++, _filename);
                        break;
                    case '{':
                        tok = new Token(TokenType.L_CURLY, _char.ToString(), _line, _column++, _filename);
                        break;
                    case '}':
                        tok = new Token(TokenType.R_CURLY, _char.ToString(), _line, _column++, _filename);
                        break;
                    case '[':
                        tok = new Token(TokenType.L_SQUARE, _char.ToString(), _line, _column++, _filename);
                        break;
                    case ']':
                        tok = new Token(TokenType.R_SQUARE, _char.ToString(), _line, _column++, _filename);
                        break;
                    case '&':
                        tok = new Token(TokenType.B_AND, _char.ToString(), _line, _column++, _filename);
                        break;
                    case '|':
                        tok = new Token(TokenType.B_OR, _char.ToString(), _line, _column++, _filename);
                        break;
                    case ';':
                        tok = new Token(TokenType.SEMICOLON, _char.ToString(), _line, _column++, _filename);
                        break;
                    case ':':
                        if ((char)Stream.Peek() == ':')
                        {
                            _char = (char)Stream.Read();
                            tok = new Token(TokenType.LABEL, "::", _line, _column, _filename);
                            _column += 2;
                        }
                        else
                        {
                            tok = new Token(TokenType.COLON, _char.ToString(), _line, _column++, _filename);
                        }
                        break;
                    case ',':
                        tok = new Token(TokenType.COMMA, _char.ToString(), _line, _column++, _filename);
                        break;
                    case '.':
                        if ((char)Stream.Peek() == '.')
                        {
                            _char = (char)Stream.Read();
                            if ((char)Stream.Peek() == '.')
                            {
                                Stream.Read();
                                tok = new Token(TokenType.VARARG, "...", _line, _column, _filename);
                                _column += 3;
                            }
                            else
                            {
                                tok = new Token(TokenType.CONCAT, "..", _line, _column, _filename);
                                _column += 2;
                            }
                        }
                        else
                        {
                            tok = new Token(TokenType.DOT, _char.ToString(), _line, _column++, _filename);
                        }

                        break;
                    case '~':
                        if ((char)Stream.Peek() == '=')
                        {
                            Stream.Read();
                            tok = new Token(TokenType.NOT_EQUAL, "~=", _line, _column, _filename);
                            _column += 2;
                        }
                        else
                        {
                            tok = new Token(TokenType.TILDE, _char.ToString(), _line, _column++, _filename);
                        }
                        break;
                    case '\"':
                    case '\'':
                        string str = ReadString();
                        tok = new Token(TokenType.STRING, str, _line, _column, _filename);
                        _column += str.Length - 1;
                        break;
                    case '\n':
                        tok = new Token(TokenType.NEWLINE, "", _line, _column, _filename);
                        _line++;
                        _column = 1;
                        break;
                    default:
                        if (char.IsLetter(_char) || _char == '_')
                        {
                            string identifier = ReadIdentifier();
                            if (_keywords.TryGetValue(identifier, out TokenType value))
                            {
                                tok = new Token(value, identifier, _line, _column, _filename);
                            }
                            else
                            {
                                tok = new Token(TokenType.IDENTIFIER, identifier, _line, _column, _filename);
                            }
                            _column += identifier.Length;
                        }
                        else if (char.IsDigit(_char))
                        {
                            string hexPattern = @"0[xX][0-9a-fA-F]+";
                            string numberString = ReadNumber();
                            if (double.TryParse(numberString, NumberStyles.Any, CultureInfo.InvariantCulture, out _)
                                || Regex.IsMatch(numberString, hexPattern))
                            {
                                tok = new Token(TokenType.NUMERICAL, numberString, _line, _column, _filename);
                                _column += numberString.Length;
                            }
                            else
                            {
                                tok = new Token(TokenType.ILLEGAL, numberString, _line, _column, _filename);
                                _column += numberString.Length;
                            }
                        }
                        else
                        {
                            string illegal = ReadIllegal();
                            tok = new Token(TokenType.ILLEGAL, illegal, _line, _column, _filename);
                        }
                        break;
                }
            }
            else
            {
                tok = new Token(TokenType.EOF, "EOF", _line, _column++, _filename);
            }
            return tok;
        }

        private void SkipWhitespace()
        {
            while (char.IsWhiteSpace(_char))
            {
                if (_char == '\n')
                {
                    break;
                }
                _column++;
                _char = (char)Stream.Read();
            }
        }

        private void SkipComment()
        {
            while (_char != '\n')
            {
                _char = (char)Stream.Read();
            }
            _column = 1;
            _line++;
        }

        private string ReadIdentifier()
        {
            StringBuilder sb = new StringBuilder(_char);
            char ch = (char)Stream.Peek();

            while (char.IsLetterOrDigit(ch) || ch == '_')
            {
                sb.Append(_char);
                _char = (char)Stream.Read();
                ch = (char)Stream.Peek();
            }

            sb.Append(_char);
            return sb.ToString();
        }

        private string ReadNumber()
        {
            StringBuilder sb = new StringBuilder(_char);
            char ch = (char)Stream.Peek();

            while (char.IsLetterOrDigit(ch) || ch == '.' || ch == '-')
            {
                sb.Append(_char);
                _char = (char)Stream.Read();
                ch = (char)Stream.Peek();
            }

            sb.Append(_char);
            return sb.ToString();
        }

        private string ReadString()
        {
            StringBuilder sb = new StringBuilder(_char);
            char closingSign = _char;
            char ch = (char)Stream.Peek();

            while (!(ch == closingSign && _char != '\\'))
            {
                sb.Append(_char);
                _char = (char)Stream.Read();
                ch = (char)Stream.Peek();
            }

            sb.Append(_char);
            sb.Append(ch);
            _char = (char)Stream.Read();
            return sb.ToString();
        }

        private string ReadIllegal()
        {
            StringBuilder sb = new StringBuilder(_char);
            char ch = (char)Stream.Peek();

            while (!char.IsWhiteSpace(ch))
            {
                sb.Append(_char);
                _char = (char)Stream.Read();
                ch = (char)Stream.Peek();
            }

            sb.Append(_char);
            return sb.ToString();
        }
    }
}