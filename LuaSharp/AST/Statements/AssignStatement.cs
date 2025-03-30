using LuaSharp.AST.Expressions;

namespace LuaSharp.AST.Statements
{
    public class AssignStatement : IStatement
    {
        public required Token Token { get; set; }
        public required IdentifierLiteral Name { get; set; }
        public required IExpression Expression { get; set; }
        public bool IsLocal { get; set; }

        public string TokenLiteral() => Token.Literal;
        public string String()
        {
            if (IsLocal)
            {
                return $"local {Name.String()} {TokenLiteral()} {Expression.String()}";
            }
            else
            {
                return $"{Name.String()} {TokenLiteral()} {Expression.String()}";
            }
        }
    }
}