using System.Text;

namespace LuaSharp
{
    public class Lexer(StreamReader stream, string filename)
    {
        public StreamReader Stream { get; set; } = stream;
        private readonly string _filename = filename;
        private char _char;
        private int _line = 1;
        private int _column = 0;
        private readonly Dictionary<string, TokenType> _keywords = new()
        {
            {"and", TokenType.AND}, {"break", TokenType.BREAK}, {"do", TokenType.DO}, {"else", TokenType.ELSE},
            {"elseif", TokenType.ELSEIF}, {"end", TokenType.END}, {"false", TokenType.FALSE}, {"for", TokenType.FOR},
            {"function", TokenType.FUNCTION}, {"if", TokenType.IF}, {"in", TokenType.IN}, {"local", TokenType.LOCAL},
            {"nil", TokenType.NIL}, {"not", TokenType.NOT}, {"or", TokenType.OR}, {"repeat", TokenType.REPEAT},
            {"return", TokenType.RETURN}, {"then", TokenType.THEN}, {"true", TokenType.TRUE},
            {"until", TokenType.UNTIL}, {"while", TokenType.WHILE}
        };

        public Token NextToken()
        {
            Token tok;
            if (!Stream.EndOfStream)
            {
                _column++;
                _char = (char)Stream.Read();

                SkipWhitespace();

                switch (_char)
                {
                    case '+':
                        tok = new Token(TokenType.PLUS, _char.ToString(), _line, _column, _filename);
                        break;
                    case '-':
                        if ((char)Stream.Peek() == '-')
                        {
                            Stream.Read();
                            tok = new Token(TokenType.COMMENT, "", _line, _column, _filename);
                            SkipComment();
                        }
                        else
                        {
                            tok = new Token(TokenType.MINUS, _char.ToString(), _line, _column, _filename);
                        }
                        break;
                    case '*':
                        tok = new Token(TokenType.ASTERISK, _char.ToString(), _line, _column, _filename);
                        break;
                    case '/':
                        tok = new Token(TokenType.SLASH, _char.ToString(), _line, _column, _filename);
                        break;
                    case '%':
                        tok = new Token(TokenType.PERCENTAGE, _char.ToString(), _line, _column, _filename);
                        break;
                    case '^':
                        tok = new Token(TokenType.CARET, _char.ToString(), _line, _column, _filename);
                        break;
                    case '#':
                        tok = new Token(TokenType.HASHTAG, _char.ToString(), _line, _column, _filename);
                        break;
                    case '<':
                        if ((char)Stream.Peek() == '=')
                        {
                            Stream.Read();
                            tok = new Token(TokenType.LESS_EQUAL, "<=", _line, _column, _filename);
                            _column++;
                        }
                        else
                        {
                            tok = new Token(TokenType.LESS, _char.ToString(), _line, _column, _filename);
                        }
                        break;
                    case '>':
                        if ((char)Stream.Peek() == '=')
                        {
                            Stream.Read();
                            tok = new Token(TokenType.MORE_EQUAL, ">=", _line, _column, _filename);
                            _column++;
                        }
                        else
                        {
                            tok = new Token(TokenType.MORE, _char.ToString(), _line, _column, _filename);
                        }
                        break;
                    case '=':
                        if ((char)Stream.Peek() == '=')
                        {
                            Stream.Read();
                            tok = new Token(TokenType.EQUAL, "==", _line, _column, _filename);
                            _column++;
                        }
                        else
                        {
                            tok = new Token(TokenType.ASSIGN, _char.ToString(), _line, _column, _filename);
                        }
                        break;
                    case '(':
                        tok = new Token(TokenType.L_PARENT, _char.ToString(), _line, _column, _filename);
                        break;
                    case ')':
                        tok = new Token(TokenType.R_PARENT, _char.ToString(), _line, _column, _filename);
                        break;
                    case '{':
                        tok = new Token(TokenType.L_CURLY, _char.ToString(), _line, _column, _filename);
                        break;
                    case '}':
                        tok = new Token(TokenType.R_CURLY, _char.ToString(), _line, _column, _filename);
                        break;
                    case '[':
                        tok = new Token(TokenType.L_SQUARE, _char.ToString(), _line, _column, _filename);
                        break;
                    case ']':
                        tok = new Token(TokenType.R_SQUARE, _char.ToString(), _line, _column, _filename);
                        break;
                    case ';':
                        tok = new Token(TokenType.SEMICOLON, _char.ToString(), _line, _column, _filename);
                        break;
                    case ':':
                        tok = new Token(TokenType.COLON, _char.ToString(), _line, _column, _filename);
                        break;
                    case ',':
                        tok = new Token(TokenType.COMMA, _char.ToString(), _line, _column, _filename);
                        break;
                    case '.':
                        if ((char)Stream.Peek() == '.')
                        {
                            _char = (char)Stream.Read();
                            if ((char)Stream.Peek() == '.')
                            {
                                Stream.Read();
                                tok = new Token(TokenType.ELLIPSIS, "...", _line, _column, _filename);
                                _column += 2;
                            }
                            else
                            {
                                tok = new Token(TokenType.CONCAT, "..", _line, _column, _filename);
                                _column++;
                            }
                        }
                        else
                        {
                            tok = new Token(TokenType.DOT, _char.ToString(), _line, _column, _filename);
                        }

                        break;
                    case '~':
                        if ((char)Stream.Peek() == '=')
                        {
                            Stream.Read();
                            tok = new Token(TokenType.NOT_EQUAL, "~=", _line, _column, _filename);
                            _column++;
                        }
                        else
                        {
                            tok = new Token(TokenType.ILLEGAL, _char.ToString(), _line, _column, _filename);
                        }
                        break;
                    case '\"':
                    case '\'':
                        string str = ReadString();
                        tok = new Token(TokenType.STRING, str, _line, _column, _filename);
                        break;
                    case '\n':
                        tok = new Token(TokenType.NEWLINE, "", _line, _column, _filename);
                        _line++;
                        _column = 0;
                        break;
                    default:
                        if (char.IsLetter(_char))
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
                            _column += identifier.Length - 1;
                        }
                        else if (char.IsDigit(_char))
                        {
                            string numberString = ReadNumber();
                            tok = new Token(TokenType.NUMERICAL, numberString, _line, _column, _filename);
                            _column += numberString.Length - 1;
                        }
                        else
                        {
                            tok = new Token(TokenType.ILLEGAL, _char.ToString(), _line, _column, _filename);
                        }
                        break;
                }
            }
            else
            {
                tok = new Token(TokenType.EOF, "EOF", _line, _column + 1, _filename);
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
            _column = 0;
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
    }
}