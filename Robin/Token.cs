namespace Robin;

public readonly struct Token(TokenType type, int start, int length)
{
    public readonly TokenType Type = type;
    public readonly int Start = start;
    public readonly int Length = length;

    public ReadOnlyMemory<char> GetValue(ReadOnlySpan<char> source)
    {
        return source.Slice(Start, Length).ToString().AsMemory();
    }

    public override string ToString()
    {
        return $"{Type} [{Start}..{Start + Length})";
    }
}