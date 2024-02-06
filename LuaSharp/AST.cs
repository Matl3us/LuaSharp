namespace LuaSharp
{
    public class AST
    {
        public List<IStatement> statements = new List<IStatement>();
        public void PrintStatements()
        {
            foreach (var st in statements)
            {
                Console.WriteLine(st.String());
            }
        }
    }

    public interface INode
    {
        public string TokenLiteral();
        public string String();
    }
    public interface IStatement : INode { };

    public struct Identifier
    {
        public TokenType token;
        public string value;
    }

    // Statements

    public struct ExpressionStatement : IStatement
    {
        public Token token;
        public string TokenLiteral() => token.Literal;
        public string String() => $"";
    }

    public struct AssignStatement : IStatement
    {
        public Token token;
        public Identifier name;

        public string TokenLiteral() => token.Literal;
        public string String() => $"{name.value} {TokenLiteral()}";
    }

    public struct ReturnStatement : IStatement
    {
        public Token token;
        public string TokenLiteral() => token.Literal;
        public string String() => $"{TokenLiteral()}";
    }
}