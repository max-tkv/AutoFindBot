using AutoFindBot.Abstractions;
using AutoFindBot.Abstractions.HttpClients;
using AutoFindBot.Entities;
using AutoFindBot.Models.TradeDealer;
using Microsoft.Extensions.Logging;

namespace AutoFindBot.Services;

public class CheckerNewAutoService : ICheckerNewAutoService
{
    private readonly ILogger<CheckerNewAutoService> _logger;
    private readonly ITradeDealerClient _tradeDealerClient;

    public CheckerNewAutoService(ILogger<CheckerNewAutoService> logger, ITradeDealerClient tradeDealerClient)
    {
        _logger = logger;
        _tradeDealerClient = tradeDealerClient;
    }

    public async Task<TradeDealerResult> GetNewAutoAsync(AppUser user)
    {
        var newAuto = await _tradeDealerClient.GetAutoByFilterAsync(new GetAutoByFilter());
        return newAuto;
    }
}