namespace Robin.Contracts.Variables;

public sealed class MemberSegment(string memberName) : IVariableSegment
{
    public string MemberName => memberName;

    public bool Accept<TArgs, TOut>(IVariableSegmentVisitor<TArgs, TOut> visitor, TArgs args, out TOut @delegate)
    {
        return visitor.VisitMember(this, args, out @delegate);
    }
}
