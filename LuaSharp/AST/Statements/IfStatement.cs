using System.Text;

namespace LuaSharp.AST.Statements
{
    public class IfStatement : IStatement
    {
        public required IExpression Condition { get; set; }
        public required BlockStatement Consequence { get; set; }
        public BlockStatement? Aternative { get; set; }

        public string TokenLiteral() => "if";
        public string String()
        {
            var ifString = new StringBuilder($"{TokenLiteral()} {Condition.String()} then {Consequence.String()} ");
            if (Aternative != null)
            {
                var alt = Aternative;
                ifString.Append($"else {alt.String()} ");
            }
            ifString.Append("end");
            return ifString.ToString();
        }
    }
}