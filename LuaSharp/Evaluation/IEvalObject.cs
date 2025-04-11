namespace LuaSharp.Evaluation
{
    public interface IEvalObject
    {
        public EvalObjectType ObjectType();
        public string InspectValue();
    }

    public enum EvalObjectType
    {
        NIL,
        INVALID,
        INTEGER,
        FLOAT,
        BOOL
    }
}