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
    public interface IExpression : INode { };

    // Expressions

    public struct Identifier : IExpression
    {
        public Token token;
        public string value;
        public string TokenLiteral() => token.Literal;
        public string String() => value;
    }

    public struct NumericalLiteral : IExpression
    {
        public Token token;
        public double value;
        public string TokenLiteral() => token.Literal;
        public string String() => token.Literal;
    }

    // Statements

    public struct ExpressionStatement : IStatement
    {
        public Token token;
        public IExpression? expression;
        public string TokenLiteral() => token.Literal;
        public string String()
        {
            if (expression != null)
            {
                return expression.String();
            }
            else
            {
                return "";
            }
        }
    }

    public struct AssignStatement : IStatement
    {
        public Token token;
        public Identifier name;

        public string TokenLiteral() => token.Literal;
        public string String() => $"{name.String()} {TokenLiteral()}";
    }

    public struct ReturnStatement : IStatement
    {
        public Token token;
        public string TokenLiteral() => token.Literal;
        public string String() => $"{TokenLiteral()}";
    }
}