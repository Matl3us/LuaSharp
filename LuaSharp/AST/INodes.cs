namespace LuaSharp.AST
{
    public interface INode
    {
        public string TokenLiteral();
        public string String();
    }

    public interface IExpression : INode { };

    public interface IStatement : INode { };
}