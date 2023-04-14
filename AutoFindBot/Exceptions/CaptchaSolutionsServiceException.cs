namespace AutoFindBot.Exceptions;

public class CaptchaSolutionsServiceException : Exception
{
    public CaptchaSolutionsServiceException()
    {
    }
    
    public CaptchaSolutionsServiceException(string message) : base(message)
    {
    }
}