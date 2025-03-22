namespace LuaSharp.AST.Expressions
{
    public struct InfixExpression : IExpression
    {
        public Token Token { get; set; }
        public string OperatorSign { get; set; }
        public IExpression LeftSide { get; set; }
        public IExpression? RightSide { get; set; }
        public readonly string TokenLiteral() => Token.Literal;
        public readonly string String()
        {
            if (RightSide != null)
            {
                return $"[{LeftSide.String()} {OperatorSign} {RightSide.String()}]";
            }
            else
            {
                return $"[{LeftSide.String()} {OperatorSign} no expression]]";
            }
        }
    }
}