namespace AutoFindBot.Integration.Invariants;

public static class RegisterTradeDealerHttpApiClientInvariants
{
    public const string OptionsSectionPath = "Integration:TradeDealer";

    public const string OptionsSectionNotDefined = "Не определена секция с настройками клиента сервиса 'TradeDealer' или эта секция пустая";

    public const string OptionNotFound = "{0} обязан иметь значение";

    public const string OptionsEmptyValue = "Конфигурация TradeDealerHttpApiClientOptions должена иметь значение";
}