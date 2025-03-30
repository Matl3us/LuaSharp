namespace LuaSharp.AST.Statements
{
    public class ReturnStatement : IStatement
    {
        public required Token Token { get; set; }
        public required IExpression Expression { get; set; }

        public string TokenLiteral() => Token.Literal;
        public string String() => $"{TokenLiteral()} {Expression.String()}";
    }
}