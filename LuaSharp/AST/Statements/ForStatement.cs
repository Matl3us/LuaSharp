namespace LuaSharp.AST.Statements
{
    public class ForStatement : IStatement
    {
        public required AssignStatement InitialValue { get; set; }
        public required IExpression Limit { get; set; }
        public required IExpression? Step { get; set; }
        public required BlockStatement Body { get; set; }

        public string TokenLiteral() => "for";
        public string String()
        {
            return $"{TokenLiteral()} [{InitialValue.String()}, {Limit.String()}, {Step?.String()}] do {Body.String()} end";
        }
    }
}