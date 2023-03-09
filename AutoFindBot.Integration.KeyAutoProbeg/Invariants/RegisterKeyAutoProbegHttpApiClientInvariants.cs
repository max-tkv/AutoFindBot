namespace AutoFindBot.Integration.KeyAutoProbeg.Invariants;

public class RegisterKeyAutoProbegHttpApiClientInvariants
{
    public const string OptionsSectionPath = "Integration:KeyAutoProbeg";

    public const string OptionsSectionNotDefined = "Не определена секция с настройками клиента сервиса 'KeyAutoProbeg' или эта секция пустая";
    
    public const string OptionNotFoundError = "{0} обязан иметь значение";
    
    public const string OptionsEmptyValue = "Конфигурация KeyAutoProbegHttpApiClientOptions должена иметь значение";
}