using AutoFindBot.Models.TradeDealer;
using AutoFindBot.Utils.Http;

namespace AutoFindBot.Abstractions.HttpClients;

public interface ITradeDealerHttpApiClient : IBaseIntegrationHttpApiClient
{
    Task<TradeDealerResult> GetAutoByFilterAsync(TradeDealerFilter filter);
}