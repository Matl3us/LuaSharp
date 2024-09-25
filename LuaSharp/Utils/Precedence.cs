namespace LuaSharp.utils
{
    public enum PrecedenceValue : int
    {
        FuncCall,
        Unary,          // not # -
        Concatenation,  // ..
        Multiplicative, // * / %
        Additive,       // + -
        Relational,     // < > <= >=
        Equality,       // == ~=
        AND,            // and
        OR,             // 	or
        Lowest
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
