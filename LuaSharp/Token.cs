namespace LuaSharp
{
    public readonly struct Token(TokenType type, string literal, int line, int column, string filename)
    {
        public TokenType Type { get; } = type;
        public string Literal { get; } = literal;
        public int Line { get; } = line;
        public int Column { get; } = column;
        public string FileName { get; } = filename;
        public override string ToString() => $"Type: {Type} Literal: {Literal} Line: {Line} Column: {Column}";
    }

    public enum TokenType
    {
        // * Keywords *
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
        REPEAT,
        RETURN,
        THEN,
        TRUE,
        UNTIL,
        WHILE,

        // * Operators *
        // - Arithmetic Operators
        PLUS,
        MINUS,
        ASTERISK,
        SLASH,
        PERCENTAGE,
        CARET,

        // - Relational Operators
        EQUAL,
        NOT_EQUAL,
        LESS_EQUAL,
        MORE_EQUAL,
        LESS,
        MORE,

        // - Logical Operators
        AND,
        OR,
        NOT,

        // - Other Operators
        HASHTAG,
        CONCAT,
        ASSIGN,

        // * Delimiters and Punctuation *
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
        ELLIPSIS,

        // * Literals *
        IDENTIFIER,
        NUMERICAL,
        STRING,

        // * Comment and Whitespace
        COMMENT,
        NEWLINE,

        // * Miscellaneous *
        ILLEGAL,
        EOF
    }
}