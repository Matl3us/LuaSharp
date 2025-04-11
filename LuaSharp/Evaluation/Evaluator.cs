using LuaSharp.AST;
using LuaSharp.AST.Expressions;
using LuaSharp.AST.Statements;
using LuaSharp.Evaluation.Objects;

namespace LuaSharp.Evaluation
{
    public static class Evaluator
    {
        public static IEvalObject Eval(INode node)
        {
            return node switch
            {
                Ast ast => EvalStatements(ast),
                ExpressionStatement expression => Eval(expression.Expression),
                IntegerNumeralLiteral intLiteral => new IntegerType { Value = intLiteral.Value },
                FloatNumeralLiteral floatLiteral => new FloatType { Value = floatLiteral.Value },
                BooleanLiteral boolLiteral => new BoolType { Value = boolLiteral.Value },
                _ => throw new NotSupportedException($"Unsupported node type: {node.GetType().Name}"),
            };
        }

        private static IEvalObject EvalStatements(Ast ast)
        {
            IEvalObject result = new InvalidType();

            foreach (var st in ast.Statements)
            {
                result = Eval(st);
            }

            return result;
        }
    }
}