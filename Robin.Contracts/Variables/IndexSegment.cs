namespace Robin.Contracts.Variables;

public sealed class IndexSegment(int index) : IVariableSegment
{
    public int Index => index;

    public bool Accept<TArgs, TOut>(IVariableSegmentVisitor<TArgs, TOut> visitor, TArgs args, out TOut @delegate)
    {
        return visitor.VisitIndex(this, args, out @delegate);
    }
}
