using System.Text;

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

    public struct BlockStatement() : IStatement
    {
        public List<IStatement> statements = [];

        public string TokenLiteral() => "";
        public string String()
        {
            var blockString = new StringBuilder();
            foreach (var statement in statements)
            {
                blockString.Append($"[{statement.String()}]");
            }
            return blockString.ToString();
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
        public IExpression expression;

        public string TokenLiteral() => token.Literal;
        public string String() => $"{TokenLiteral()} {expression.String()}";
    }

    public struct IfStatement : IStatement
    {
        public IExpression condition;
        public BlockStatement consequence;
        public BlockStatement? alternative;

        public string TokenLiteral() => "if";
        public string String()
        {
            var ifString = new StringBuilder($"{TokenLiteral()} {condition.String()} then {consequence.String()} ");
            if (alternative != null)
            {
                var alt = (BlockStatement)alternative;
                ifString.Append($"else {alt.String()} ");
            }
            ifString.Append("end");
            return ifString.ToString();
        }
    }

    public struct WhileStatement : IStatement
    {
        public IExpression condition;
        public BlockStatement body;

        public string TokenLiteral() => "while";
        public string String()
        {
            return $"{TokenLiteral()} {condition.String()} do {body.String()} end";
        }
    }

    public struct ForStatement : IStatement
    {
        public AssignStatement initialValue;
        public IExpression limit;
        public IExpression step;
        public BlockStatement body;

        public string TokenLiteral() => "for";
        public string String()
        {
            return $"{TokenLiteral()} [{initialValue.String()}, {limit.String()}, {step.String()}] do {body.String()} end";
        }
    }

    public struct RepeatStatement : IStatement
    {
        public IExpression condition;
        public BlockStatement body;

        public string TokenLiteral() => "repeat";
        public string String()
        {
            return $"{TokenLiteral()} {body.String()} until {condition.String()}";
        }
    }

    public struct FunctionStatement : IStatement
    {
        public Identifier name;
        public List<Identifier> parameters;
        public BlockStatement body;

        public string TokenLiteral() => "function";
        public string String()
        {
            var paramsString = new StringBuilder();
            foreach (var parameter in parameters)
            {
                paramsString.Append($"[{parameter.String()}]");
            }
            return $"{TokenLiteral()} {name.String()}[{paramsString}] {body.String()} end";
        }
    }
}