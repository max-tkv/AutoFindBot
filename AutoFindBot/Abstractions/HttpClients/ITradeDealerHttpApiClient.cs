using AutoFindBot.Models.TradeDealer;

namespace AutoFindBot.Abstractions.HttpClients;

public interface ITradeDealerHttpApiClient
{
    Task<TradeDealerResult> GetAllNewAutoAsync(CancellationToken stoppingToken = default);
}