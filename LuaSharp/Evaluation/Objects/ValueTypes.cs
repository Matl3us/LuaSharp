namespace LuaSharp.Evaluation.Objects
{
    public abstract class ValueType<T> : IEvalObject
    {
        public required T Value { get; set; }

        public string InspectValue() => Value?.ToString() ?? string.Empty;
        public abstract EvalObjectType ObjectType();
    }

    public class IntegerType : ValueType<int>
    {
        public override EvalObjectType ObjectType() => EvalObjectType.INTEGER;
    }

    public class FloatType : ValueType<double>
    {
        public override EvalObjectType ObjectType() => EvalObjectType.FLOAT;
    }

    public class BoolType : ValueType<bool>
    {
        public override EvalObjectType ObjectType() => EvalObjectType.BOOL;
    }
}