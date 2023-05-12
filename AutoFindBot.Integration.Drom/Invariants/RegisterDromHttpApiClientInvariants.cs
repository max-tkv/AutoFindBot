namespace AutoFindBot.Integration.Drom.Invariants;

public class RegisterDromHttpApiClientInvariants
{
    public const string OptionsSectionPath = "Integration:Drom";

    public const string OptionsSectionNotDefined = "Не определена секция с настройками клиента сервиса 'Drom' или эта секция пустая";
    
    public const string OptionNotFoundError = "{0} обязан иметь значение";
    
    public const string OptionsEmptyValue = "Конфигурация DromHttpApiClientOptions должена иметь значение";
}