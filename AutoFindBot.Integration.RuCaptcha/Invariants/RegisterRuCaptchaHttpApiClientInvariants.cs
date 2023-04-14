namespace AutoFindBot.Integration.RuCaptcha.Invariants;

public class RegisterRuCaptchaHttpApiClientInvariants
{
    public const string OptionsSectionPath = "Integration:RuCaptcha";

    public const string OptionsSectionNotDefined = "Не определена секция с настройками клиента сервиса 'RuCaptcha' или эта секция пустая";
    
    public const string OptionNotFoundError = "{0} обязан иметь значение";
    
    public const string OptionsEmptyValue = "Конфигурация RuCaptchaHttpApiClientOptions должена иметь значение";
}