namespace LuaSharp.AST
{
    public class Ast
    {
        public List<IStatement> Statements { get; set; } = [];
        public void PrintStatements()
        {
            foreach (var st in Statements)
            {
                Console.WriteLine(st.String());
            }
        }
    }
}