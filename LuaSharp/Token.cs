namespace LuaSharp
{
    public struct Token
    {
        public Token(TokenType type, string literal)
        {
            Type = type;
            Literal = literal;
        }
        public TokenType Type { get; }
        public string Literal { get; }
        public override string ToString() => $"Type: {Type} Literal: {Literal}";
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