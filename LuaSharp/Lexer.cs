namespace LuaSharp
{
    public class Lexer
    {
        public Lexer(StreamReader stream)
        {
            Stream = stream;
        }
        public StreamReader Stream { get; set; }
        private char _char;
        private int _line = 1;
        private int _column = 0;

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
                        tok = new Token(TokenType.Plus, _char.ToString(), _line, _column);
                        break;
                    case '-':
                        tok = new Token(TokenType.Minus, _char.ToString(), _line, _column);
                        break;
                    case '*':
                        tok = new Token(TokenType.Asterisk, _char.ToString(), _line, _column);
                        break;
                    case '/':
                        tok = new Token(TokenType.Slash, _char.ToString(), _line, _column);
                        break;
                    case '%':
                        tok = new Token(TokenType.Percentage, _char.ToString(), _line, _column);
                        break;
                    case '^':
                        tok = new Token(TokenType.Caret, _char.ToString(), _line, _column);
                        break;
                    case '#':
                        tok = new Token(TokenType.Hashtag, _char.ToString(), _line, _column);
                        break;
                    case '<':
                        if ((char)Stream.Peek() == '=')
                        {
                            Stream.Read();
                            tok = new Token(TokenType.LessEqual, "<=", _line, _column);
                            _column++;
                        }
                        else
                        {
                            tok = new Token(TokenType.Less, _char.ToString(), _line, _column);
                        }
                        break;
                    case '>':
                        if ((char)Stream.Peek() == '=')
                        {
                            Stream.Read();
                            tok = new Token(TokenType.MoreEqual, ">=", _line, _column);
                            _column++;
                        }
                        else
                        {
                            tok = new Token(TokenType.More, _char.ToString(), _line, _column);
                        }
                        break;
                    case '=':
                        if ((char)Stream.Peek() == '=')
                        {
                            Stream.Read();
                            tok = new Token(TokenType.Equal, "==", _line, _column);
                            _column++;
                        }
                        else
                        {
                            tok = new Token(TokenType.Assign, _char.ToString(), _line, _column);
                        }
                        break;
                    case '(':
                        tok = new Token(TokenType.LParent, _char.ToString(), _line, _column);
                        break;
                    case ')':
                        tok = new Token(TokenType.RParent, _char.ToString(), _line, _column);
                        break;
                    case '{':
                        tok = new Token(TokenType.LCurly, _char.ToString(), _line, _column);
                        break;
                    case '}':
                        tok = new Token(TokenType.RCurly, _char.ToString(), _line, _column);
                        break;
                    case '[':
                        tok = new Token(TokenType.LSquare, _char.ToString(), _line, _column);
                        break;
                    case ']':
                        tok = new Token(TokenType.RSquare, _char.ToString(), _line, _column);
                        break;
                    case ';':
                        tok = new Token(TokenType.Semicolon, _char.ToString(), _line, _column);
                        break;
                    case ':':
                        tok = new Token(TokenType.Colon, _char.ToString(), _line, _column);
                        break;
                    case ',':
                        tok = new Token(TokenType.Comma, _char.ToString(), _line, _column);
                        break;
                    case '.':
                        if ((char)Stream.Peek() == '.')
                        {
                            _char = (char)Stream.Read();
                            if ((char)Stream.Peek() == '.')
                            {
                                Stream.Read();
                                tok = new Token(TokenType.Ellipsis, "...", _line, _column);
                                _column += 2;
                            }
                            else
                            {
                                tok = new Token(TokenType.Concat, "..", _line, _column);
                                _column++;
                            }
                        }
                        else
                        {
                            tok = new Token(TokenType.Dot, _char.ToString(), _line, _column);
                        }

                        break;
                    case '~':
                        if ((char)Stream.Peek() == '=')
                        {
                            Stream.Read();
                            tok = new Token(TokenType.NotEqual, "~=", _line, _column);
                            _column++;
                        }
                        else
                        {
                            tok = new Token(TokenType.ILLEGAL, _char.ToString(), _line, _column);
                        }
                        break;
                    default:
                        tok = new Token(TokenType.ILLEGAL, _char.ToString(), _line, _column);
                        break;
                }
            }
            else
            {
                tok = new Token(TokenType.EOF, "EOF", _line, _column);
            }
            return tok;
        }

        private void SkipWhitespace()
        {
            while (char.IsWhiteSpace(_char))
            {
                if (_char == '\n')
                {
                    _line++;
                    _column = 1;
                }
                else
                {
                    _column++;
                }

                _char = (char)Stream.Read();
            }
        }
    }
}