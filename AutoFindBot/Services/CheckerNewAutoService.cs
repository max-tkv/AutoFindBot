using AutoFindBot.Abstractions;
using AutoFindBot.Abstractions.HttpClients;
using AutoFindBot.Entities;
using AutoFindBot.Models.TradeDealer;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace AutoFindBot.Services;

public class CheckerNewAutoService : ICheckerNewAutoService
{
    private readonly ILogger<CheckerNewAutoService> _logger;
    private readonly ITradeDealerClient _tradeDealerClient;
    private readonly IMapper _mapper;

    public CheckerNewAutoService(
        ILogger<CheckerNewAutoService> logger, 
        ITradeDealerClient tradeDealerClient, 
        IMapper mapper)
    {
        _logger = logger;
        _tradeDealerClient = tradeDealerClient;
        _mapper = mapper;
    }

    public async Task<TradeDealerResult> GetNewAutoAsync(AppUser user, UserFilter userFilter)
    {
        var getAutoByFilter = _mapper.Map<GetAutoByFilter>(userFilter);
        var newAuto = await _tradeDealerClient.GetAutoByFilterAsync(getAutoByFilter);
        return newAuto;
    }
}