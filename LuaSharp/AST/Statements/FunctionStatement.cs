using System.Text;
using LuaSharp.AST.Expressions;

namespace LuaSharp.AST.Statements
{
    public class FunctionStatement : IStatement
    {
        public required IdentifierLiteral Name { get; set; }
        public required List<IdentifierLiteral> Parameters { get; set; }
        public required BlockStatement Body { get; set; }

        public string TokenLiteral() => "function";
        public string String()
        {
            var paramsString = new StringBuilder();
            foreach (var parameter in Parameters)
            {
                paramsString.Append($"[{parameter.String()}]");
            }
            return $"{TokenLiteral()} {Name.String()}[{paramsString}] {Body.String()} end";
        }
    }
}