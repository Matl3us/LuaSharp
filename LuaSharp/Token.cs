namespace LuaSharp
{
    public struct Token
    {
        public Token(TokenType type, string literal, int line, int column)
        {
            Type = type;
            Literal = literal;
            Line = line;
            Column = column;
        }
        public TokenType Type { get; }
        public string Literal { get; }
        public int Line { get; }
        public int Column { get; }
        public override string ToString() => $"Type: {Type} Literal: {Literal} Line: {Line} Column: {Column}";
    }

    public enum TokenType
    {
        // Keywords
        AND,
        BREAK,
        DO,
        ELSE,
        ELSEIF,
        END,
        FALSE,
        FOR,
        FUNCTION,
        IF,
        IN,
        LOCAL,
        NIL,
        NOT,
        OR,
        REPEAT,
        RETURN,
        THEN,
        TRUE,
        UNTIL,
        WHILE,
        // Operators
        PLUS,
        MINUS,
        ASTERISK,
        SLASH,
        PERCENTAGE,
        CARET,
        HASHTAG,
        EQUAL,
        NOT_EQUAL,
        LESS_EQUAL,
        MORE_EQUAL,
        LESS,
        MORE,
        ASSIGN,
        L_PARENT,
        R_PARENT,
        L_CURLY,
        R_CURLY,
        L_SQUARE,
        R_SQUARE,
        SEMICOLON,
        COLON,
        COMMA,
        DOT,
        CONCAT,
        ELLIPSIS,

        // Rest
        IDENTIFIER,
        NUMERICAL,
        ILLEGAL,
        EOF
    }
}