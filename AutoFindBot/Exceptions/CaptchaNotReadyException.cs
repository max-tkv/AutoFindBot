namespace AutoFindBot.Exceptions;

public class CaptchaNotReadyException : Exception
{
    public CaptchaNotReadyException(string message) : base(message)
    {
    }
}