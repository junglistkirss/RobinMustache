namespace Robin.Contracts.Variables;

public sealed class ThisSegment : IVariableSegment
{
    public readonly static ThisSegment Instance = new();
    public bool Accept<TArgs, TOut>(IVariableSegmentVisitor<TArgs, TOut> visitor, TArgs args, out TOut @delegate)
    {
        return visitor.VisitThis(this, args, out @delegate);
    }
}
