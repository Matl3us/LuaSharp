using System.Text;

namespace LuaSharp
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

    public interface INode
    {
        public string TokenLiteral();
        public string String();
    }
    public interface IStatement : INode { };
    public interface IExpression : INode { };

    // Expressions
    public abstract class Literal<T> : IExpression
    {
        public Token Token { get; set; }
        public required T Value { get; set; }

        public string TokenLiteral() => Token.Literal;
        public string String() => Token.Literal;
    }

    public class IdentifierLiteral : Literal<string> { }

    public class IntegerNumeralLiteral : Literal<int> { }

    public class FloatNumeralLiteral : Literal<double> { }

    public class BooleanLiteral : Literal<bool> { }

    public struct PrefixExpression : IExpression
    {
        public Token Token { get; set; }
        public string OperatorSign { get; set; }
        public IExpression? RightSide { get; set; }
        public readonly string TokenLiteral() => Token.Literal;
        public readonly string String()
        {
            if (RightSide != null)
            {
                return $"[{OperatorSign} {RightSide.String()}]";
            }
            else
            {
                return $"[{OperatorSign} no expression]";
            }
        }
    }

    public struct InfixExpression : IExpression
    {
        public Token Token { get; set; }
        public string OperatorSign { get; set; }
        public IExpression LeftSide { get; set; }
        public IExpression? RightSide { get; set; }
        public readonly string TokenLiteral() => Token.Literal;
        public readonly string String()
        {
            if (RightSide != null)
            {
                return $"[{LeftSide.String()} {OperatorSign} {RightSide.String()}]";
            }
            else
            {
                return $"[{LeftSide.String()} {OperatorSign} no expression]]";
            }
        }
    }

    // Statements

    public struct ExpressionStatement : IStatement
    {
        public Token Token { get; set; }
        public IExpression? Expression { get; set; }
        public readonly string TokenLiteral() => Token.Literal;
        public readonly string String()
        {
            if (Expression != null)
            {
                return Expression.String();
            }
            else
            {
                return "";
            }
        }
    }

    public struct BlockStatement() : IStatement
    {
        public List<IStatement> Statements { get; set; } = [];

        public readonly string TokenLiteral() => "";
        public readonly string String()
        {
            var blockString = new StringBuilder();
            foreach (var statement in Statements)
            {
                blockString.Append($"[{statement.String()}]");
            }
            return blockString.ToString();
        }
    }

    public struct AssignStatement : IStatement
    {
        public Token Token { get; set; }
        public IdentifierLiteral Name { get; set; }
        public IExpression Expression { get; set; }
        public bool IsLocal { get; set; }

        public readonly string TokenLiteral() => Token.Literal;
        public readonly string String()
        {
            if (IsLocal)
            {
                return $"local {Name.String()} {TokenLiteral()} {Expression.String()}";
            }
            else
            {
                return $"{Name.String()} {TokenLiteral()} {Expression.String()}";
            }
        }
    }

    public struct ReturnStatement : IStatement
    {
        public Token Token { get; set; }
        public IExpression Expression { get; set; }

        public readonly string TokenLiteral() => Token.Literal;
        public readonly string String() => $"{TokenLiteral()} {Expression.String()}";
    }

    public struct IfStatement : IStatement
    {
        public IExpression Condition { get; set; }
        public BlockStatement Consequence { get; set; }
        public BlockStatement? Aternative { get; set; }

        public readonly string TokenLiteral() => "if";
        public readonly string String()
        {
            var ifString = new StringBuilder($"{TokenLiteral()} {Condition.String()} then {Consequence.String()} ");
            if (Aternative != null)
            {
                var alt = (BlockStatement)Aternative;
                ifString.Append($"else {alt.String()} ");
            }
            ifString.Append("end");
            return ifString.ToString();
        }
    }

    public struct WhileStatement : IStatement
    {
        public IExpression Condition { get; set; }
        public BlockStatement Body { get; set; }

        public readonly string TokenLiteral() => "while";
        public readonly string String()
        {
            return $"{TokenLiteral()} {Condition.String()} do {Body.String()} end";
        }
    }

    public struct ForStatement : IStatement
    {
        public AssignStatement InitialValue { get; set; }
        public IExpression Limit { get; set; }
        public IExpression Step { get; set; }
        public BlockStatement Body { get; set; }

        public readonly string TokenLiteral() => "for";
        public readonly string String()
        {
            return $"{TokenLiteral()} [{InitialValue.String()}, {Limit.String()}, {Step.String()}] do {Body.String()} end";
        }
    }

    public struct RepeatStatement : IStatement
    {
        public IExpression Condition { get; set; }
        public BlockStatement Body { get; set; }

        public readonly string TokenLiteral() => "repeat";
        public readonly string String()
        {
            return $"{TokenLiteral()} {Body.String()} until {Condition.String()}";
        }
    }

    public struct FunctionStatement : IStatement
    {
        public IdentifierLiteral Name { get; set; }
        public List<IdentifierLiteral> Parameters { get; set; }
        public BlockStatement Body { get; set; }

        public readonly string TokenLiteral() => "function";
        public readonly string String()
        {
            var paramsString = new StringBuilder();
            foreach (var parameter in Parameters)
            {
                paramsString.Append($"[{parameter.String()}]");
            }
            return $"{TokenLiteral()} {Name.String()}[{paramsString}] {Body.String()} end";
        }
    }

    public struct FunctionCallStatement : IStatement
    {
        public IdentifierLiteral Name { get; set; }
        public List<IExpression> Arguments { get; set; }

        public readonly string TokenLiteral() => "";
        public readonly string String()
        {
            var argsString = new StringBuilder();
            foreach (var arg in Arguments)
            {
                argsString.Append($"[{arg.String()}]");
            }
            return $"{Name.String()}[{argsString}]";
        }
    }
}