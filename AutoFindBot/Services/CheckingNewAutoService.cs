using AutoFindBot.Abstractions;
using AutoFindBot.Abstractions.HttpClients;
using AutoFindBot.Entities;
using AutoFindBot.Models.TradeDealer;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace AutoFindBot.Services;

public class CheckingNewAutoService : ICheckingNewAutoService
{
    private readonly ILogger<CheckingNewAutoService> _logger;
    private readonly ITradeDealerClient _tradeDealerClient;
    private readonly IUserFilterService _userFilterService;
    private readonly IMessageService _messageService;
    private readonly ICarService _carService;
    private readonly IMapper _mapper;

    public CheckingNewAutoService(
        ILogger<CheckingNewAutoService> logger, 
        ITradeDealerClient tradeDealerClient,
        IUserFilterService userFilterService,
        IMessageService messageService,
        ICarService carService,
        IMapper mapper)
    {
        _logger = logger;
        _tradeDealerClient = tradeDealerClient;
        _userFilterService = userFilterService;
        _messageService = messageService;
        _carService = carService;
        _mapper = mapper;
    }

    public async Task CheckAndSendMessageAsync(TelegramBotClient botClient, AppUser user, bool sendEmptyResultMessage = false)
    {
        var filters = await _userFilterService.GetByUserAsync(user);
        _logger.LogInformation($"User ID: {user.Id}. Find {filters.Count} filters.");
        
        foreach (var filter in filters)
        {
            _logger.LogInformation($"User ID: {user.Id}. Select Filter ID: {filter.Id}");
                    
            var newAutoList = await GetAutoByFilterAsync(filter);
            var newCars = await _carService.GetNewCarsAndSaveAsync(newAutoList.CarInfos, user, filter);
            if (newCars.Any() || sendEmptyResultMessage)
            {
                await _messageService.SendNewAutoMessageAsync(botClient, user, filter, newCars);   
            }
                
            _logger.LogInformation($"Find new cars: {newCars.Count}");
        }
        
    }
    private async Task<TradeDealerResult> GetAutoByFilterAsync(UserFilter userFilter)
    {
        var getAutoByFilter = _mapper.Map<GetAutoByFilter>(userFilter);
        return await _tradeDealerClient.GetAutoByFilterAsync(getAutoByFilter);
    }
}