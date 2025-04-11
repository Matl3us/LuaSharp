namespace LuaSharp.Evaluation.Objects
{
    public class InvalidType : IEvalObject
    {
        public string InspectValue() => "INVALID";
        public EvalObjectType ObjectType() => EvalObjectType.INVALID;
    }
}
