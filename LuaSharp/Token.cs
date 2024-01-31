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
        Plus,
        Minus,
        Asterisk,
        Slash,
        Percentage,
        Caret,
        Hashtag,
        Equal,
        NotEqual,
        LessEqual,
        MoreEqual,
        Less,
        More,
        Assign,
        LParent,
        RParent,
        LCurly,
        RCurly,
        LSquare,
        RSquare,
        Semicolon,
        Colon,
        Comma,
        Dot,
        Concat,
        Ellipsis,

        ILLEGAL,
        EOF
    }
}