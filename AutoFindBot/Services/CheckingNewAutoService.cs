using AutoFindBot.Abstractions;
using AutoFindBot.Abstractions.HttpClients;
using AutoFindBot.Entities;
using AutoFindBot.Models.AutoRu;
using AutoFindBot.Models.Avito;
using AutoFindBot.Models.KeyAutoProbeg;
using AutoFindBot.Models.TradeDealer;
using AutoFindBot.Repositories;
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
    private readonly IAutoRuHttpApiClient _autoRuHttpApiClient;
    private readonly IHistorySourceCheckService _historySourceCheckService;

    public CheckingNewAutoService(
        ILogger<CheckingNewAutoService> logger,
        IHistorySourceCheckService historySourceCheckService,
        IKeyAutoProbegHttpApiClient keyAutoProbegHttpApiClient,
        ITradeDealerHttpApiClient tradeDealerHttpApiClient,
        IAutoRuHttpApiClient autoRuHttpApiClient,
        IAvitoHttpApiClient avitoHttpApiClient,
        IUserFilterService userFilterService,
        IMessageService messageService,
        ICarService carService,
        IMapper mapper)
    {
        _logger = logger;
        _historySourceCheckService = historySourceCheckService;
        _keyAutoProbegHttpApiClient = keyAutoProbegHttpApiClient;
        _tradeDealerHttpApiClient = tradeDealerHttpApiClient;
        _autoRuHttpApiClient = autoRuHttpApiClient;
        _avitoHttpApiClient = avitoHttpApiClient;
        _userFilterService = userFilterService;
        _messageService = messageService;
        _carService = carService;
        _mapper = mapper;
    }

    public async Task CheckAndSendMessageAsync(
        TelegramBotClient botClient, 
        AppUser user, 
        bool sendEmptyResultMessage = false)
    {
        var filters = await _userFilterService.GetByUserAsync(user);
        _logger.LogInformation($"User ID: {user.Id}. Find {filters.Count} filters.");
        
        var tasks = new List<Task>();
        foreach (var filter in filters)
        {
            _logger.LogInformation($"User ID: {user.Id}. Select Filter ID: {filter.Id}");
            tasks.Add(Task.Run(() => GetAutoAndSendMessageByFilterAsync(botClient, user, filter, sendEmptyResultMessage)));
        }
        await Task.WhenAll(tasks);
    }

    private async Task GetAutoAndSendMessageByFilterAsync(
        TelegramBotClient botClient, 
        AppUser user, 
        UserFilter filter, 
        bool sendEmptyResultMessage)
    {
        var newAutoList = await GetAutoByFilterAsync(filter);
        if (newAutoList.Any() || sendEmptyResultMessage)
        {
            await _messageService.SendNewAutoMessageAsync(botClient, user, filter, newAutoList);   
        }
        
        _logger.LogInformation($"User ID: {user.Id}. Filter ID: {filter.Id}. New cars found: {newAutoList.Count}");
    }

    private async Task<List<Car>> GetAutoByFilterAsync(UserFilter userFilter)
    {
        var cars = new List<Car>();

        await GetCarsFromAutoRuAsync(userFilter, cars);
        await GetCarsFromTradeDealerAsync(userFilter, cars);
        await GetCarsFromKeyAutoProbegAsync(userFilter, cars);
        await GetCarsFromAvitoAsync(userFilter, cars);

        return cars;
    }

    private async Task GetCarsFromAvitoAsync(UserFilter filter, List<Car> cars)
    {
        try
        {
            var avitoFilter = _mapper.Map<AvitoFilter>(filter);
            var avitoResult = await _avitoHttpApiClient.GetAutoByFilterAsync(avitoFilter);
            var carList = _mapper.Map<List<Car>>(avitoResult);

            var existsSuccess = await _historySourceCheckService.ExistsSuccessBySourceAsync(filter, Source.Avito);
            var historySourceCheckId = await _historySourceCheckService.AddSuccessAsync(filter, Source.Avito);
            if (existsSuccess)
            {
                var newCars = await _carService.GetNewCarsAndSaveAsync(carList, filter, historySourceCheckId);
                cars.AddRange(newCars);
            }
            else
            {
                await _carService.GetNewCarsAndSaveAsync(carList, filter, historySourceCheckId);
            }
        }
        catch (Exception e)
        {
            await _historySourceCheckService.AddErrorAsync(filter, Source.Avito, e.Message);
        }
    }
    
    private async Task GetCarsFromKeyAutoProbegAsync(UserFilter filter, List<Car> cars)
    {
        try
        {
            var keyAutoProbeg = _mapper.Map<KeyAutoProbegFilter>(filter);
            var keyAutoProbegResult = await _keyAutoProbegHttpApiClient.GetAutoByFilterAsync(keyAutoProbeg);
            var carList = _mapper.Map<List<Car>>(keyAutoProbegResult);

            var existsSuccess = await _historySourceCheckService.ExistsSuccessBySourceAsync(filter, Source.KeyAutoProbeg);
            var historySourceCheckId = await _historySourceCheckService.AddSuccessAsync(filter, Source.KeyAutoProbeg);
            if (existsSuccess)
            {
                var newCars = await _carService.GetNewCarsAndSaveAsync(carList, filter, historySourceCheckId);
                cars.AddRange(newCars);
            }
            else
            {
                await _carService.GetNewCarsAndSaveAsync(carList, filter, historySourceCheckId);
            }
        }
        catch (Exception e)
        {
            await _historySourceCheckService.AddErrorAsync(filter, Source.KeyAutoProbeg, e.Message);
        }
    }
    
    private async Task GetCarsFromTradeDealerAsync(UserFilter filter, List<Car> cars)
    {
        try
        {
            var tradeDealerFilter = _mapper.Map<TradeDealerFilter>(filter);
            var tradeDealerResult = await _tradeDealerHttpApiClient.GetAutoByFilterAsync(tradeDealerFilter);
            var carList = _mapper.Map<List<Car>>(tradeDealerResult.CarInfos);
            
            var existsSuccess = await _historySourceCheckService.ExistsSuccessBySourceAsync(filter, Source.TradeDealer);
            var historySourceCheckId = await _historySourceCheckService.AddSuccessAsync(filter, Source.TradeDealer);
            if (existsSuccess)
            {
                var newCars = await _carService.GetNewCarsAndSaveAsync(carList, filter, historySourceCheckId);
                cars.AddRange(newCars);
            }
            else
            {
                await _carService.GetNewCarsAndSaveAsync(carList, filter, historySourceCheckId);
            }
        }
        catch (Exception e)
        {
            await _historySourceCheckService.AddErrorAsync(filter, Source.TradeDealer, e.Message);
        }
    }
    
    private async Task GetCarsFromAutoRuAsync(UserFilter filter, List<Car> cars)
    {
        try
        {
            var autoRuFilter = _mapper.Map<AutoRuFilter>(filter);
            var autoRuResult = await _autoRuHttpApiClient.GetAutoByFilterAsync(autoRuFilter);
            var carList = _mapper.Map<List<Car>>(autoRuResult.Offers);

            var existsSuccess = await _historySourceCheckService.ExistsSuccessBySourceAsync(filter, Source.AutoRu);
            var historySourceCheckId = await _historySourceCheckService.AddSuccessAsync(filter, Source.AutoRu);
            if (existsSuccess)
            {
                var newCars = await _carService.GetNewCarsAndSaveAsync(carList, filter, historySourceCheckId);
                cars.AddRange(newCars);
            }
            else
            {
                await _carService.GetNewCarsAndSaveAsync(carList, filter, historySourceCheckId);
            }
        }
        catch (Exception e)
        {
            await _historySourceCheckService.AddErrorAsync(filter, Source.AutoRu, e.Message);
        }
    }
}