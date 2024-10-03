namespace LuaSharp
{
    public class AST
    {
        public List<IStatement> statements = [];
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

    public struct IntegerNumeralLiteral : IExpression
    {
        public Token token;
        public int value;
        public string TokenLiteral() => token.Literal;
        public string String() => token.Literal;
    }

    public struct FloatNumeralLiteral : IExpression
    {
        public Token token;
        public double value;
        public string TokenLiteral() => token.Literal;
        public string String() => token.Literal;
    }

    public struct BooleanLiteral : IExpression
    {
        public Token token;
        public bool value;
        public string TokenLiteral() => token.Literal;
        public string String() => token.Literal;
    }

    public struct PrefixExpression : IExpression
    {
        public Token token;
        public string operatorSign;
        public IExpression? rightSide;
        public string TokenLiteral() => token.Literal;
        public string String()
        {
            if (rightSide != null)
            {
                return $"[{operatorSign} {rightSide.String()}]";
            }
            else
            {
                return $"[{operatorSign} no expression]";
            }
        }
    }

    public struct InfixExpression : IExpression
    {
        public Token token;
        public string operatorSign;
        public IExpression leftSide;
        public IExpression? rightSide;
        public string TokenLiteral() => token.Literal;
        public string String()
        {
            if (rightSide != null)
            {
                return $"[{leftSide.String()} {operatorSign} {rightSide.String()}]";
            }
            else
            {
                return $"[{leftSide.String()} {operatorSign} no expression]]";
            }
        }
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
        public IExpression expression;
        public bool isLocal;

        public string TokenLiteral() => token.Literal;
        public string String()
        {
            if (isLocal)
            {
                return $"local {name.String()} {TokenLiteral()} {expression.String()}";
            }
            else
            {
                return $"{name.String()} {TokenLiteral()} {expression.String()}";
            }
        }
    }

    public struct ReturnStatement : IStatement
    {
        public Token token;
        public string TokenLiteral() => token.Literal;
        public string String() => $"{TokenLiteral()}";
    }
}