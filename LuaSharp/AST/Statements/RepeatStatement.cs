namespace LuaSharp.AST.Statements
{
    public class RepeatStatement : IStatement
    {
        public required IExpression Condition { get; set; }
        public required BlockStatement Body { get; set; }

        public string TokenLiteral() => "repeat";
        public string String()
        {
            return $"{TokenLiteral()} {Body.String()} until {Condition.String()}";
        }
    }
}