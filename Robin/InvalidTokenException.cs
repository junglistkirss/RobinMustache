namespace Robin;

public sealed class InvalidTokenException : Exception
{
    public InvalidTokenException(string message) : base(message) { }
}