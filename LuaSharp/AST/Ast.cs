namespace LuaSharp.AST
{
    public class Ast : INode
    {
        public List<IStatement> Statements { get; set; } = [];
        public void PrintStatements()
        {
            foreach (var st in Statements)
            {
                Console.WriteLine(st.String());
            }
        }

        public string String()
        {
            throw new NotImplementedException();
        }

        public string TokenLiteral()
        {
            throw new NotImplementedException();
        }
    }
}