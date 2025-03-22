namespace LuaSharp.AST.Statements
{
    public class WhileStatement : IStatement
    {
        public required IExpression Condition { get; set; }
        public required BlockStatement Body { get; set; }

        public string TokenLiteral() => "while";
        public string String()
        {
            return $"{TokenLiteral()} {Condition.String()} do {Body.String()} end";
        }
    }
}