namespace AutoFindBot.Integration.Youla.Invariants;

public class RegisterYoulaHttpApiClientInvariants
{
    public const string OptionsSectionPath = "Integration:Youla";

    public const string OptionsSectionNotDefined = "Не определена секция с настройками клиента сервиса 'Youla' или эта секция пустая";
    
    public const string OptionNotFoundError = "{0} обязан иметь значение";
    
    public const string OptionsEmptyValue = "Конфигурация YoulaHttpApiClientOptions должена иметь значение";
}