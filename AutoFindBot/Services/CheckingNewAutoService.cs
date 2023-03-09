using AutoFindBot.Abstractions;
using AutoFindBot.Abstractions.HttpClients;
using AutoFindBot.Entities;
using AutoFindBot.Models.Avito;
using AutoFindBot.Models.KeyAutoProbeg;
using AutoFindBot.Models.TradeDealer;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace AutoFindBot.Services;

public class CheckingNewAutoService : ICheckingNewAutoService
{
    private readonly ILogger<CheckingNewAutoService> _logger;
    private readonly ITradeDealerHttpApiClient _tradeDealerHttpApiClient;
    private readonly IUserFilterService _userFilterService;
    private readonly IMessageService _messageService;
    private readonly ICarService _carService;
    private readonly IMapper _mapper;
    private readonly IKeyAutoProbegHttpApiClient _keyAutoProbegHttpApiClient;
    private readonly IAvitoHttpApiClient _avitoHttpApiClient;

    public CheckingNewAutoService(
        ILogger<CheckingNewAutoService> logger,
        IKeyAutoProbegHttpApiClient keyAutoProbegHttpApiClient,
        ITradeDealerHttpApiClient tradeDealerHttpApiClient,
        IAvitoHttpApiClient avitoHttpApiClient,
        IUserFilterService userFilterService,
        IMessageService messageService,
        ICarService carService,
        IMapper mapper)
    {
        _logger = logger;
        _keyAutoProbegHttpApiClient = keyAutoProbegHttpApiClient;
        _tradeDealerHttpApiClient = tradeDealerHttpApiClient;
        _avitoHttpApiClient = avitoHttpApiClient;
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
            var newCars = await _carService.GetNewCarsAndSaveAsync(newAutoList, user, filter);
            if (newCars.Any() || sendEmptyResultMessage)
            {
                await _messageService.SendNewAutoMessageAsync(botClient, user, filter, newCars);   
            }
                
            _logger.LogInformation($"Find new cars: {newCars.Count}");
        }
        
    }
    private async Task<List<Car>> GetAutoByFilterAsync(UserFilter userFilter)
    {
        var cars = new List<Car>();
        
        var tradeDealerFilter = _mapper.Map<TradeDealerFilter>(userFilter);
        var tradeDealerResult = await _tradeDealerHttpApiClient.GetAutoByFilterAsync(tradeDealerFilter);
        cars.AddRange(_mapper.Map<List<Car>>(tradeDealerResult.CarInfos));
        
        var keyAutoProbeg = _mapper.Map<KeyAutoProbegFilter>(userFilter);
        var keyAutoProbegResult = await _keyAutoProbegHttpApiClient.GetAutoByFilterAsync(keyAutoProbeg);
        cars.AddRange(_mapper.Map<List<Car>>(keyAutoProbegResult));
        
        var avitoFilter = _mapper.Map<AvitoFilter>(userFilter);
        var avitoResult = await _avitoHttpApiClient.GetAutoByFilterAsync(avitoFilter);
        cars.AddRange(_mapper.Map<List<Car>>(avitoResult));
        
        return cars;
    }
}