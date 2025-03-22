using System.Text;

namespace LuaSharp.AST.Statements
{
    public class BlockStatement() : IStatement
    {
        public List<IStatement> Statements { get; set; } = [];

        public string TokenLiteral() => "";
        public string String()
        {
            var blockString = new StringBuilder();
            foreach (var statement in Statements)
            {
                blockString.Append($"[{statement.String()}]");
            }
            return blockString.ToString();
        }
    }
}