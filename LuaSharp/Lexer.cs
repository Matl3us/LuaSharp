namespace LuaSharp
{
    public class Lexer
    {
        public Lexer(StreamReader stream)
        {
            Stream = stream;
        }
        public StreamReader Stream { get; set; }

        public Token NextToken()
        {
            Token tok = default;

            if (!Stream.EndOfStream)
            {
                char ch = (char)Stream.Read();

                switch (ch)
                {
                    case '+':
                        tok = new Token(TokenType.Plus, ch.ToString());
                        break;
                    case '-':
                        tok = new Token(TokenType.Minus, ch.ToString());
                        break;
                    case '*':
                        tok = new Token(TokenType.Asterisk, ch.ToString());
                        break;
                    case '/':
                        tok = new Token(TokenType.Slash, ch.ToString());
                        break;
                    case '%':
                        tok = new Token(TokenType.Percentage, ch.ToString());
                        break;
                    case '^':
                        tok = new Token(TokenType.Caret, ch.ToString());
                        break;
                    case '#':
                        tok = new Token(TokenType.Hashtag, ch.ToString());
                        break;
                    case '<':
                        tok = new Token(TokenType.Less, ch.ToString());
                        break;
                    case '>':
                        tok = new Token(TokenType.More, ch.ToString());
                        break;
                    case '=':
                        tok = new Token(TokenType.Assign, ch.ToString());
                        break;
                    case '(':
                        tok = new Token(TokenType.LParent, ch.ToString());
                        break;
                    case ')':
                        tok = new Token(TokenType.RParent, ch.ToString());
                        break;
                    case '{':
                        tok = new Token(TokenType.LCurly, ch.ToString());
                        break;
                    case '}':
                        tok = new Token(TokenType.RCurly, ch.ToString());
                        break;
                    case '[':
                        tok = new Token(TokenType.LSquare, ch.ToString());
                        break;
                    case ']':
                        tok = new Token(TokenType.RSquare, ch.ToString());
                        break;
                    case ';':
                        tok = new Token(TokenType.Semicolon, ch.ToString());
                        break;
                    case ':':
                        tok = new Token(TokenType.Colon, ch.ToString());
                        break;
                    case ',':
                        tok = new Token(TokenType.Comma, ch.ToString());
                        break;
                    case '.':
                        tok = new Token(TokenType.Dot, ch.ToString());
                        break;
                    default:
                        tok = new Token(TokenType.ILLEGAL, ch.ToString());
                        break;
                }
            }
            else
            {
                tok = new Token(TokenType.EOF, "EOF");
            }
            return tok;
        }
    }
}