namespace LuaSharp
{
    public readonly struct Token(TokenType type, string literal, int line, int column, string filename)
    {
        public TokenType Type { get; } = type;
        public string Literal { get; } = literal;
        public int Line { get; } = line;
        public int Column { get; } = column;
        public string FileName { get; } = filename;
        public override string ToString() => $"[{Type},{Literal},{Line},{Column}]";
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
        GOTO,
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

        // - Bitwise Operators
        B_AND,
        B_OR,
        B_NOT,
        B_RSHIFT,
        B_LSHIFT,

        // - Other Operators
        HASHTAG,
        CONCAT,
        ASSIGN,
        F_DIV,
        LABEL,
        VARARG,

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