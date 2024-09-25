namespace LuaSharp.utils
{
    public enum PrecedenceValue : int
    {
        Lowest,
        OR,              // or
        AND,             // and
        Equality,        // == ~=
        Relational,      // < > <= >=
        BitwiseOr,       // |
        BitwiseXor,      // ~
        BitwiseAnd,      // &
        Shift,           // << >>
        Concatenation,   // ..
        Additive,        // + -
        Multiplicative,  // * / // %
        Unary,           // not # - ~
        Exponentiation,  // ^
        FuncCall,
    }

    public static class Precedence
    {
        private static readonly Dictionary<TokenType, PrecedenceValue> precedenceMap = new()
        {
            {TokenType.ASTERISK, PrecedenceValue.Multiplicative}, {TokenType.SLASH, PrecedenceValue.Multiplicative},
            {TokenType.PLUS, PrecedenceValue.Additive}, {TokenType.MINUS, PrecedenceValue.Additive},
            {TokenType.LESS, PrecedenceValue.Relational}, {TokenType.MORE, PrecedenceValue.Relational},
            {TokenType.LESS_EQUAL, PrecedenceValue.Relational}, {TokenType.MORE_EQUAL, PrecedenceValue.Relational},
            {TokenType.EQUAL, PrecedenceValue.Equality}, {TokenType.NOT_EQUAL, PrecedenceValue.Equality}
        };

        public static int PeekPrecedence(Token token)
        {
            if (precedenceMap.TryGetValue(token.Type, out PrecedenceValue value))
            {
                return (int)value;
            }

            return (int)PrecedenceValue.Lowest;
        }

        public static int CurPrecedence(Token token)
        {
            if (precedenceMap.TryGetValue(token.Type, out PrecedenceValue value))
            {
                return (int)value;
            }

            return (int)PrecedenceValue.Lowest;
        }
    }
}
