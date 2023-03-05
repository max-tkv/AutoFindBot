using AutoFindBot.Entities;
using AutoFindBot.Models.TradeDealer;

namespace AutoFindBot.Abstractions;

public interface ICheckerNewAutoService
{
    Task<TradeDealerResult> GetNewAutoAsync(AppUser user);
}