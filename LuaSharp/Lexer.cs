using System.Text;
using System.Text.RegularExpressions;

namespace LuaSharp
{
    public class Lexer(StreamReader stream, string filename = "console")
    {
        private readonly StreamReader _stream = stream;
        private readonly string _filename = filename;
        private char _curChar;
        private char _nextChar;
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

        private enum LexerState
        {
            Start,
            ReadingNumber,
            ReadingString,
            ReadingIdentifier,
            ReadingComment,
        }

        public Token NextToken()
        {
            var token = new Token()
                .SetLine(_line)
                .SetColumn(_column)
                .SetFileName(_filename);
            var state = LexerState.Start;
            var buffer = new StringBuilder();

            while (!_stream.EndOfStream)
            {
                switch (state)
                {
                    case LexerState.Start:
                        ReadChar();
                        if (char.IsWhiteSpace(_curChar))
                        {
                            if (_curChar == '\n')
                            {
                                return ReadNewLine(token);
                            }
                            else if (_curChar == '\r')
                            {
                                break;
                            }
                            _column++;
                            token.SetColumn(_column);
                        }
                        else if (char.IsLetter(_curChar) || _curChar == '_')
                        {
                            state = LexerState.ReadingIdentifier;
                            buffer.Append(_curChar);
                        }
                        else if (char.IsDigit(_curChar))
                        {
                            state = LexerState.ReadingNumber;
                            buffer.Append(_curChar);
                        }
                        else if (_curChar == '\"' || _curChar == '\'')
                        {
                            state = LexerState.ReadingString;
                            buffer.Append(_curChar);
                        }
                        else if (_curChar == '-' && _nextChar == '-')
                        {
                            state = LexerState.ReadingComment;
                            ReadChar();
                        }
                        else
                        {
                            _column++;
                            return ReadSymbol(token);
                        }

                        break;
                    case LexerState.ReadingIdentifier:
                        if (char.IsLetterOrDigit(_nextChar) || _nextChar == '_')
                        {
                            ReadChar();
                            buffer.Append(_curChar);
                        }
                        else
                        {
                            return CreateIdentifier(buffer, token);
                        }
                        break;
                    case LexerState.ReadingNumber:
                        if (char.IsLetterOrDigit(_nextChar)
                            || _nextChar == '.' || _nextChar == '-'
                            || _nextChar == 'x' || _nextChar == 'X'
                            || _nextChar == 'e' || _nextChar == 'E')
                        {
                            ReadChar();
                            buffer.Append(_curChar);
                        }
                        else
                        {
                            return CreateNumber(buffer, token);
                        }
                        break;
                    case LexerState.ReadingString:
                        ReadChar();
                        buffer.Append(_curChar);
                        if (_curChar == buffer[0] && buffer.Length > 1 && buffer[^2] != '\\')
                        {
                            return CreateString(buffer, token);
                        }
                        break;
                    case LexerState.ReadingComment:
                        ReadChar();
                        state = LexerState.Start;
                        break;
                }
            }

            return state switch
            {
                LexerState.Start => token.SetLiteral("EOF").SetType(TokenType.EOF),
                LexerState.ReadingIdentifier => CreateIdentifier(buffer, token),
                LexerState.ReadingNumber => CreateNumber(buffer, token),
                LexerState.ReadingString => CreateString(buffer, token),
                _ => token.SetLiteral("ILLEGAL").SetType(TokenType.ILLEGAL),
            };
        }

        private void ReadChar()
        {
            _curChar = (char)_stream.Read();
            _nextChar = (char)_stream.Peek();
        }

        private Token CreateIdentifier(StringBuilder buffer, Token token)
        {
            string identifier = buffer.ToString();
            var len = identifier.Length;
            TokenType type = _keywords.TryGetValue(identifier, out TokenType value) ? value : TokenType.IDENTIFIER;
            token.SetType(type).SetLiteral(identifier);
            _column += len;
            return token;
        }

        private Token CreateNumber(StringBuilder buffer, Token token)
        {
            string numberString = buffer.ToString();
            var len = numberString.Length;
            string integerPattern = @"\b(0[xX][A-Fa-f0-9]+|\d+)\b";
            string floatPattern = @"\b(?:\d+\.\d+|\d+)(?:[eE][+-]?\d+)?\b";
            _ = (Regex.IsMatch(numberString, integerPattern)
                || Regex.IsMatch(numberString, floatPattern)) ?
                token.SetType(TokenType.NUMERICAL).SetLiteral(numberString) :
                token.SetType(TokenType.ILLEGAL).SetLiteral(numberString);
            _column += len;
            return token;
        }

        private Token CreateString(StringBuilder buffer, Token token)
        {
            string stringLiteral = buffer.ToString();
            var len = stringLiteral.Length;
            TokenType type = (_curChar == buffer[0] && buffer.Length > 1 && buffer[^2] != '\\') ?
                TokenType.STRING : TokenType.ILLEGAL;
            token.SetType(type).SetLiteral(stringLiteral);
            _column += len;
            return token;
        }

        private Token ReadSymbol(Token token)
        {
            var literal = _curChar.ToString();
            token.SetLiteral(literal);

            return _curChar switch
            {
                '+' => token.SetType(TokenType.PLUS),
                '-' => token.SetType(TokenType.MINUS),
                '*' => token.SetType(TokenType.ASTERISK),
                '/' => PeekAndReadSlash(token),
                '%' => token.SetType(TokenType.PERCENTAGE),
                '^' => token.SetType(TokenType.CARET),
                '#' => token.SetType(TokenType.HASHTAG),
                '<' => PeekAndReadLessThan(token),
                '>' => PeekAndReadGreaterThan(token),
                '=' => PeekAndReadEquals(token),
                '(' => token.SetType(TokenType.L_PARENT),
                ')' => token.SetType(TokenType.R_PARENT),
                '{' => token.SetType(TokenType.L_CURLY),
                '}' => token.SetType(TokenType.R_CURLY),
                '[' => token.SetType(TokenType.L_SQUARE),
                ']' => token.SetType(TokenType.R_SQUARE),
                '&' => token.SetType(TokenType.B_AND),
                '|' => token.SetType(TokenType.B_OR),
                ';' => token.SetType(TokenType.SEMICOLON),
                ':' => PeekAndReadColon(token),
                ',' => token.SetType(TokenType.COMMA),
                '.' => PeekAndReadPeriod(token),
                '~' => PeekAndReadTilde(token),
                _ => token.SetType(TokenType.ILLEGAL)
            };
        }

        private Token PeekAndReadSlash(Token token)
        {
            if (_nextChar != '/')
            {
                return token.SetType(TokenType.SLASH);
            }
            ReadChar();
            token.SetType(TokenType.F_DIV).SetLiteral("//");
            _column++;
            return token;
        }

        private Token PeekAndReadLessThan(Token token)
        {
            if (_nextChar == '=')
            {
                ReadChar();
                token.SetType(TokenType.LESS_EQUAL).SetLiteral("<=");
                _column++;
                return token;
            }
            else if (_nextChar == '<')
            {
                ReadChar();
                token.SetType(TokenType.B_LSHIFT).SetLiteral("<<");
                _column++;
                return token;
            }
            return token.SetType(TokenType.LESS);
        }

        private Token PeekAndReadGreaterThan(Token token)
        {
            if (_nextChar == '=')
            {
                ReadChar();
                token.SetType(TokenType.MORE_EQUAL).SetLiteral(">=");
                _column++;
                return token;
            }
            else if (_nextChar == '>')
            {
                ReadChar();
                token.SetType(TokenType.B_RSHIFT).SetLiteral(">>");
                _column++;
                return token;
            }
            return token.SetType(TokenType.MORE);
        }

        private Token PeekAndReadEquals(Token token)
        {
            if (_nextChar == '=')
            {
                ReadChar();
                token.SetType(TokenType.EQUAL).SetLiteral("==");
                _column++;
                return token;
            }
            return token.SetType(TokenType.ASSIGN);
        }

        private Token PeekAndReadColon(Token token)
        {
            if (_nextChar == ':')
            {
                ReadChar();
                token.SetType(TokenType.LABEL).SetLiteral("::");
                _column++;
                return token;
            }
            return token.SetType(TokenType.COLON);
        }

        private Token PeekAndReadPeriod(Token token)
        {
            if (_nextChar == '.')
            {
                ReadChar();
                if (_nextChar == '.')
                {
                    ReadChar();
                    token.SetType(TokenType.VARARG).SetLiteral("...");
                    _column += 2;
                    return token;
                }
                else
                {
                    token.SetType(TokenType.CONCAT).SetLiteral("..");
                    _column++;
                    return token;
                }
            }
            return token.SetType(TokenType.DOT);
        }

        private Token PeekAndReadTilde(Token token)
        {
            if (_nextChar == '=')
            {
                ReadChar();
                token.SetType(TokenType.NOT_EQUAL).SetLiteral("~=");
                _column++;
                return token;
            }
            return token.SetType(TokenType.TILDE);
        }

        private Token ReadNewLine(Token token)
        {
            token.SetType(TokenType.NEWLINE).SetLiteral("");
            _line++;
            _column = 1;
            return token;
        }
    }
}