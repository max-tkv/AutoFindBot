namespace AutoFindBot.Integration.Youla.Invariants;

public class YoulaHttpApiClientInvariants
{
    public const string CaptchaNotReadyMessage = "Капча еще не решена.";
    
    public const string HttpErrorMessage = "Произошла ошибка при вызове метода :requestUri. " +
                                           "Получен ответ: :content";

    public const string PriceMin = ":priceMin";
    
    public const string PriceMax = ":priceMax";
}