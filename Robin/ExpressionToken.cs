namespace Robin;

public readonly struct ExpressionToken(ExpressionType type, int start, int length)
{
    public ExpressionType Type => type;
    public int Start => start;
    public int Length => length;

    public ReadOnlyMemory<char> GetValue(ReadOnlySpan<char> source)
    {
        return source.Slice(Start, Length).ToString().AsMemory();
    }

    public override string ToString()
    {
        return $"{Type} [{Start}..{Start + Length})";
    }
}