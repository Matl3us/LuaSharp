namespace LuaSharp.utils
{
    public enum PrecedenceValue
    {
        Lowest,
        OR,              // or
        AND,             // and
        Equality,        // == ~=
        Relational,      // < > <= >=
        BitwiseOr,       // |
        BitwiseNot,      // ~
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
            {TokenType.OR, PrecedenceValue.OR}, {TokenType.AND, PrecedenceValue.AND},
            {TokenType.EQUAL, PrecedenceValue.Equality}, {TokenType.NOT_EQUAL, PrecedenceValue.Equality},
            {TokenType.LESS, PrecedenceValue.Relational}, {TokenType.MORE, PrecedenceValue.Relational},
            {TokenType.LESS_EQUAL, PrecedenceValue.Relational}, {TokenType.MORE_EQUAL, PrecedenceValue.Relational},
            {TokenType.B_OR, PrecedenceValue.BitwiseOr}, {TokenType.TILDE, PrecedenceValue.BitwiseNot},
            {TokenType.B_AND, PrecedenceValue.BitwiseAnd}, {TokenType.B_RSHIFT, PrecedenceValue.Shift},
            {TokenType.B_LSHIFT, PrecedenceValue.Shift}, {TokenType.CONCAT, PrecedenceValue.Concatenation},
            {TokenType.PLUS, PrecedenceValue.Additive}, {TokenType.MINUS, PrecedenceValue.Additive},
            {TokenType.ASTERISK, PrecedenceValue.Multiplicative}, {TokenType.SLASH, PrecedenceValue.Multiplicative},
            {TokenType.F_DIV, PrecedenceValue.Multiplicative}, {TokenType.PERCENTAGE, PrecedenceValue.Multiplicative},
            {TokenType.CARET, PrecedenceValue.Exponentiation}, {TokenType.L_PARENT, PrecedenceValue.FuncCall}
        };

        public static int PeekPrecedence(Token token)
        {
            if (precedenceMap.TryGetValue(token.Type, out PrecedenceValue value))
            {
                return (int)value;
            }

            return (int)PrecedenceValue.Lowest;
        }
    }
}
