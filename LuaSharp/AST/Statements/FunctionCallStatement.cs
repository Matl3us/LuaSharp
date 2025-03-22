using System.Text;
using LuaSharp.AST.Expressions;

namespace LuaSharp.AST.Statements
{
    public class FunctionCallStatement : IStatement
    {
        public required IdentifierLiteral Name { get; set; }
        public required List<IExpression> Arguments { get; set; }

        public string TokenLiteral() => "";
        public string String()
        {
            var argsString = new StringBuilder();
            foreach (var arg in Arguments)
            {
                argsString.Append($"[{arg.String()}]");
            }
            return $"{Name.String()}[{argsString}]";
        }
    }
}