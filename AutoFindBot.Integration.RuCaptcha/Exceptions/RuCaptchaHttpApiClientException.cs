namespace AutoFindBot.Integration.RuCaptcha.Exceptions;

public class RuCaptchaHttpApiClientException : Exception
{
    public RuCaptchaHttpApiClientException(string message) : base(message)
    {
    }
}