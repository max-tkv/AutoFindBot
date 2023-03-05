using AutoFindBot.Entities;
using AutoFindBot.Models.TradeDealer;

namespace AutoFindBot.Abstractions;

public interface ICheckerNewAutoService
{
    Task<TradeDealerResult> GetAutoByFilterAsync(AppUser user, UserFilter userFilter);
}