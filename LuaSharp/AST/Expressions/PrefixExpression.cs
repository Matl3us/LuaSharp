namespace LuaSharp.AST.Expressions
{
    public struct PrefixExpression : IExpression
    {
        public Token Token { get; set; }
        public string OperatorSign { get; set; }
        public IExpression? RightSide { get; set; }
        public readonly string TokenLiteral() => Token.Literal;
        public readonly string String()
        {
            if (RightSide != null)
            {
                return $"[{OperatorSign} {RightSide.String()}]";
            }
            else
            {
                return $"[{OperatorSign} no expression]";
            }
        }
    }
}