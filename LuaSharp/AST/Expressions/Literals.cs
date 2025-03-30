namespace LuaSharp.AST.Expressions
{
    public abstract class Literal<T> : IExpression
    {
        public required Token Token { get; set; }
        public required T Value { get; set; }

        public string TokenLiteral() => Token.Literal;
        public string String() => Token.Literal;
    }

    public class IdentifierLiteral : Literal<string> { }

    public class IntegerNumeralLiteral : Literal<int> { }

    public class FloatNumeralLiteral : Literal<double> { }

    public class BooleanLiteral : Literal<bool> { }
}