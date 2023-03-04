namespace AutoFindBot.Exceptions;

public class MessageServiceException : Exception
{
    public MessageServiceException(string message) : base(message)
    {
    }
}