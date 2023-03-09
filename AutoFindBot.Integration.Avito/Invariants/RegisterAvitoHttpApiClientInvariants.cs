namespace AutoFindBot.Integration.Avito.Invariants;

public class RegisterAvitoHttpApiClientInvariants
{
    public const string OptionsSectionPath = "Integration:Avito";

    public const string OptionsSectionNotDefined = "Не определена секция с настройками клиента сервиса 'Avito' или эта секция пустая";
    
    public const string OptionNotFoundError = "{0} обязан иметь значение";
    
    public const string OptionsEmptyValue = "Конфигурация AvitoHttpApiClientOptions должена иметь значение";
}