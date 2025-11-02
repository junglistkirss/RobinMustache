namespace Robin;

public record struct RenderResult(bool IsComplete, Exception? Exception)
{
    public readonly static RenderResult Complete = new(true, null);
    public static RenderResult Fail(Exception? exception = null) => new(false, exception);
};

