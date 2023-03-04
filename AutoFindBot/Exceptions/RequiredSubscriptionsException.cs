namespace AutoFindBot.Exceptions;

public class RequiredSubscriptionsException : Exception
{
    public RequiredSubscriptionsException(string message) : base(message)
    {
    }
}