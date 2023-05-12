namespace AutoFindBot.Exceptions;

public class NotFoundWebElementException : Exception
{
    public NotFoundWebElementException(string message) : base(message)
    {
    }
}