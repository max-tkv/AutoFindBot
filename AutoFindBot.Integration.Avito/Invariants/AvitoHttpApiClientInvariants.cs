namespace AutoFindBot.Integration.Avito.Invariants;

public class AvitoHttpApiClientInvariants
{
    public const string PriceMin = ":priceMin";
    
    public const string PriceMax = ":priceMax";

    public const string HttpErrorMessage = "Произошла ошибка при вызове метода :requestUri. " +
                                           "Получен ответ: :content";

    public const string SuccessFlag = "ok";

    public const string CaptchaFlag = "too-many-requests";
}