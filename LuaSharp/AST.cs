namespace LuaSharp
{
    public class AST
    {
        public List<IStatement> statements = new List<IStatement>();
        public void PrintStatements()
        {
            foreach (var st in statements)
            {
                Console.WriteLine(st.TokenLiteral());
            }
        }
    }
    public interface INode
    {
        public string TokenLiteral();
    }
    public interface IStatement : INode { };

    public struct Identifier
    {
        public TokenType token;
        public string value;
    }

    public struct AssignStatement : IStatement
    {
        public Identifier name;

        public string TokenLiteral()
        {
            return $"Literal: {name.value}";
        }
    }
}