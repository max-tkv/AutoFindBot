using AutoFindBot.Models.TradeDealer;
using AutoFindBot.Utils.Http;

namespace AutoFindBot.Abstractions.HttpClients;

public interface ITradeDealerHttpApiClient
{
    Task<TradeDealerResult> GetAutoByFilterAsync(TradeDealerFilter filter);
}