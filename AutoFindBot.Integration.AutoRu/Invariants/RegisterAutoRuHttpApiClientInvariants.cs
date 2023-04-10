namespace AutoFindBot.Integration.AutoRu.Invariants;

public class RegisterAutoRuHttpApiClientInvariants
{
    public const string OptionsSectionPath = "Integration:AutoRu";

    public const string OptionsSectionNotDefined = "Не определена секция с настройками клиента сервиса 'AutoRu' или эта секция пустая";
    
    public const string OptionNotFoundError = "{0} обязан иметь значение";
    
    public const string OptionsEmptyValue = "Конфигурация AutoRuHttpApiClientOptions должена иметь значение";
}