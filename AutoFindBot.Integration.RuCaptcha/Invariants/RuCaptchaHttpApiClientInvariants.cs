namespace AutoFindBot.Integration.RuCaptcha.Invariants;

public class RuCaptchaHttpApiClientInvariants
{
    public const string ApiKey = ":apiKey";
    
    public const string Id = ":id";

    public const string CaptchaNotReady = "CAPCHA_NOT_READY";
    
    public const string CaptchaNotReadyMessage = "Капча еще не решена.";
    
    public const string HttpErrorMessage = "Произошла ошибка при вызове метода :requestUri. " +
                                           "Получен ответ: :content";
}