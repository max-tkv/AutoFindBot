using AutoFindBot.Models.TradeDealer;

namespace AutoFindBot.Abstractions.HttpClients;

public interface ITradeDealerClient
{
    Task<TradeDealerResult> GetAutoByFilterAsync(GetAutoByFilter filter);
}