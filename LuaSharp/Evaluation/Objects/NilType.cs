namespace LuaSharp.Evaluation.Objects
{
    public class NilType : IEvalObject
    {
        public string InspectValue() => "NIL";
        public EvalObjectType ObjectType() => EvalObjectType.NIL;
    }
}