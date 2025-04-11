namespace LuaSharp.AST.Statements
{
    public class ExpressionStatement : IStatement
    {
        public required Token Token { get; set; }
        public required IExpression Expression { get; set; }
        public string TokenLiteral() => Token.Literal;
        public string String()
        {
            if (Expression != null)
            {
                return Expression.String();
            }
            else
            {
                return "";
            }
        }
    }
}