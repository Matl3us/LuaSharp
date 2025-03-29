using System;
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
                                return ReadNewLine();
                            }
                            else if(_curChar == '\r')
                            {
                                break;
                            }
                            _column++;
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
                            return ReadSymbol();
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
                            return CreateIdentifier(buffer);
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
                            return CreateNumber(buffer);
                        }
                        break;
                    case LexerState.ReadingString:
                        ReadChar();
                        buffer.Append(_curChar);
                        if (_curChar == buffer[0] && buffer[^2] != '\\')
                        {
                            return CreateString(buffer);
                        }
                        break;
                    case LexerState.ReadingComment:
                        ReadChar();
                        state = LexerState.Start;
                        break;
                }
            }

            var token = state switch
            {
                LexerState.Start => new Token(TokenType.EOF, "EOF", _line, _column, _filename),
                LexerState.ReadingIdentifier => CreateIdentifier(buffer),
                LexerState.ReadingNumber => CreateNumber(buffer),
                LexerState.ReadingString => CreateString(buffer),
                _ => new Token(TokenType.EOF, "EOF", _line, _column, _filename),
            };
            return token;
        }

        private void ReadChar()
        {
            _curChar = (char)_stream.Read();
            _nextChar = (char)_stream.Peek();
        }

        private Token CreateIdentifier(StringBuilder buffer)
        {
            string identifier = buffer.ToString();
            var len = identifier.Length;
            TokenType type = _keywords.TryGetValue(identifier, out TokenType value) ? value : TokenType.IDENTIFIER;
            var token = new Token(type, identifier, _line, _column, _filename);
            _column += len;
            return token;
        }

        private Token CreateNumber(StringBuilder buffer)
        {
            string numberString = buffer.ToString();
            var len = numberString.Length;
            string integerPattern = @"\b(0[xX][A-Fa-f0-9]+|\d+)\b";
            string floatPattern = @"\b(?:\d+\.\d+|\d+)(?:[eE][+-]?\d+)?\b";
            var token = (Regex.IsMatch(numberString, integerPattern)
                || Regex.IsMatch(numberString, floatPattern)) ?
                new Token(TokenType.NUMERICAL, numberString, _line, _column, _filename) :
                new Token(TokenType.ILLEGAL, numberString, _line, _column, _filename);
            _column += len;
            return token;
        }

        private Token CreateString(StringBuilder buffer)
        {
            string stringLiteral = buffer.ToString();
            var len = stringLiteral.Length;
            TokenType type = (_curChar == buffer[0] && buffer[^2] != '\\') ?
                TokenType.STRING : TokenType.ILLEGAL;
            var token = new Token(type, stringLiteral, _line, _column, _filename);
            _column += len;
            return token;
        }

        private Token ReadSymbol() => _curChar switch
        {
            '+' => new Token(TokenType.PLUS, _curChar.ToString(), _line, _column++, _filename),
            '-' => new Token(TokenType.MINUS, _curChar.ToString(), _line, _column++, _filename),
            '*' => new Token(TokenType.ASTERISK, _curChar.ToString(), _line, _column++, _filename),
            '/' => PeekAndReadSlash(),
            '%' => new Token(TokenType.PERCENTAGE, _curChar.ToString(), _line, _column++, _filename),
            '^' => new Token(TokenType.CARET, _curChar.ToString(), _line, _column++, _filename),
            '#' => new Token(TokenType.HASHTAG, _curChar.ToString(), _line, _column++, _filename),
            '<' => PeekAndReadLessThan(),
            '>' => PeekAndReadGreaterThan(),
            '=' => PeekAndReadEquals(),
            '(' => new Token(TokenType.L_PARENT, _curChar.ToString(), _line, _column++, _filename),
            ')' => new Token(TokenType.R_PARENT, _curChar.ToString(), _line, _column++, _filename),
            '{' => new Token(TokenType.L_CURLY, _curChar.ToString(), _line, _column++, _filename),
            '}' => new Token(TokenType.R_CURLY, _curChar.ToString(), _line, _column++, _filename),
            '[' => new Token(TokenType.L_SQUARE, _curChar.ToString(), _line, _column++, _filename),
            ']' => new Token(TokenType.R_SQUARE, _curChar.ToString(), _line, _column++, _filename),
            '&' => new Token(TokenType.B_AND, _curChar.ToString(), _line, _column++, _filename),
            '|' => new Token(TokenType.B_OR, _curChar.ToString(), _line, _column++, _filename),
            ';' => new Token(TokenType.SEMICOLON, _curChar.ToString(), _line, _column++, _filename),
            ':' => PeekAndReadColon(),
            ',' => new Token(TokenType.COMMA, _curChar.ToString(), _line, _column++, _filename),
            '.' => PeekAndReadPeriod(),
            '~' => PeekAndReadTilde(),
            _ => new Token(TokenType.ILLEGAL, _curChar.ToString(), _line, _column, _filename)
        };

        private Token PeekAndReadSlash()
        {
            if ((char)_stream.Peek() != '/')
            {
                return new Token(TokenType.SLASH, _curChar.ToString(), _line, _column++, _filename);
            }
            ReadChar();
            var token = new Token(TokenType.F_DIV, "//", _line, _column, _filename);
            _column += 2;
            return token;
        }

        private Token PeekAndReadLessThan()
        {
            var peek = (char)_stream.Peek();
            if (peek == '=')
            {
                ReadChar();
                var token = new Token(TokenType.LESS_EQUAL, "<=", _line, _column, _filename);
                _column += 2;
                return token;
            }
            else if (peek == '<')
            {
                ReadChar();
                var token = new Token(TokenType.B_LSHIFT, "<<", _line, _column, _filename);
                _column += 2;
                return token;
            }
            else
            {
                return new Token(TokenType.LESS, _curChar.ToString(), _line, _column++, _filename);
            }
        }

        private Token PeekAndReadGreaterThan()
        {
            var peek = (char)_stream.Peek();
            if (peek == '=')
            {
                ReadChar();
                var token = new Token(TokenType.MORE_EQUAL, ">=", _line, _column, _filename);
                _column += 2;
                return token;
            }
            else if (peek == '>')
            {
                ReadChar();
                var token = new Token(TokenType.B_RSHIFT, ">>", _line, _column, _filename);
                _column += 2;
                return token;
            }
            else
            {
                return new Token(TokenType.MORE, _curChar.ToString(), _line, _column++, _filename);
            }
        }

        private Token PeekAndReadEquals()
        {
            var peek = (char)_stream.Peek();
            if (peek == '=')
            {
                ReadChar();
                var token = new Token(TokenType.EQUAL, "==", _line, _column, _filename);
                _column += 2;
                return token;
            }
            else
            {
                return new Token(TokenType.ASSIGN, _curChar.ToString(), _line, _column++, _filename);
            }
        }

        private Token PeekAndReadColon()
        {
            var peek = (char)_stream.Peek();
            if (peek == ':')
            {
                ReadChar();
                var token = new Token(TokenType.LABEL, "::", _line, _column, _filename);
                _column += 2;
                return token;
            }
            else
            {
                return new Token(TokenType.COLON, _curChar.ToString(), _line, _column++, _filename);
            }
        }

        private Token PeekAndReadPeriod()
        {
            var peek = (char)_stream.Peek();
            if (peek == '.')
            {
                ReadChar();
                if ((char)_stream.Peek() == '.')
                {
                    ReadChar();
                    var token = new Token(TokenType.VARARG, "...", _line, _column, _filename);
                    _column += 3;
                    return token;
                }
                else
                {
                   var token = new Token(TokenType.CONCAT, "..", _line, _column, _filename);
                    _column += 2;
                    return token;
                }
            }
            else
            {
                return new Token(TokenType.DOT, _curChar.ToString(), _line, _column++, _filename);
            }
        }

        private Token PeekAndReadTilde()
        {
            var peek = (char)_stream.Peek();
            if (peek == '=')
            {
                ReadChar();
                var token = new Token(TokenType.NOT_EQUAL, "~=", _line, _column, _filename);
                _column += 2;
                return token;
            }
            else
            {
                return new Token(TokenType.TILDE, _curChar.ToString(), _line, _column++, _filename);
            }
        }

        private Token ReadNewLine()
        {
            var token = new Token(TokenType.NEWLINE, "", _line, _column, _filename);
            _line++;
            _column = 1;
            return token;
        }
    }
}