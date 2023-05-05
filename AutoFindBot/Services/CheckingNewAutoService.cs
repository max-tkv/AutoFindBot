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
    private readonly ISourceCheckService _historySourceCheckService;

    public CheckingNewAutoService(
        ILogger<CheckingNewAutoService> logger,
        ISourceCheckService historySourceCheckService,
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
        var newAutoList = await GetAutoByFilterAsync(filter, botClient, user);
        if (newAutoList.Any() || sendEmptyResultMessage)
        {
            await _messageService.SendNewAutoMessageAsync(botClient, user, filter, newAutoList);   
        }
        
        _logger.LogInformation(
            $"User ID: {user.Id}. " +
            $"Filter ID: {filter.Id}. " +
            $"New cars found: {newAutoList.Count}");
    }

    private async Task<List<Car>> GetAutoByFilterAsync(
        UserFilter userFilter, 
        TelegramBotClient botClient, 
        AppUser user)
    {
        var cars = new List<Car>();

        await GetCarsFromAutoRuAsync(userFilter, cars, botClient, user);
        await GetCarsFromTradeDealerAsync(userFilter, cars, botClient, user);
        await GetCarsFromKeyAutoProbegAsync(userFilter, cars, botClient, user);
        await GetCarsFromAvitoAsync(userFilter, cars, botClient, user);

        return cars;
    }

    private async Task GetCarsFromAvitoAsync(
        UserFilter filter, 
        List<Car> cars, 
        TelegramBotClient botClient, 
        AppUser user)
    {
        try
        {
            var avitoFilter = _mapper.Map<AvitoFilter>(filter);
            var avitoResult = await _avitoHttpApiClient.GetAutoByFilterAsync(avitoFilter);
            var carList = _mapper.Map<List<Car>>(avitoResult);

            var existsSuccess = await _historySourceCheckService.ExistsAsync(filter, Source.Avito);
            if (existsSuccess)
            {
                var newCars = await _carService.GetNewCarsAndSaveAsync(carList, filter);
                cars.AddRange(newCars);
            }
            else
            {
                await _carService.GetNewCarsAndSaveAsync(carList, filter);
                await _historySourceCheckService.AddSourceAsync(filter, Source.Avito);
                
                var existsAny = await _historySourceCheckService.ExistsAsync(filter);
                if (!existsAny)
                {
                    await _messageService.SendUserConfirmationMessageAsync(botClient, user);   
                }
            }
        }
        catch (Exception e)
        {
            _logger.LogWarning(
                $"User Filter ID: {filter.Id}. " +
                $"Method GetCarsFromAvitoAsync: {e.Message}");
        }
    }
    
    private async Task GetCarsFromKeyAutoProbegAsync(
        UserFilter filter, 
        List<Car> cars, 
        TelegramBotClient botClient, 
        AppUser user)
    {
        try
        {
            var keyAutoProbeg = _mapper.Map<KeyAutoProbegFilter>(filter);
            var keyAutoProbegResult = await _keyAutoProbegHttpApiClient.GetAutoByFilterAsync(keyAutoProbeg);
            var carList = _mapper.Map<List<Car>>(keyAutoProbegResult);

            var existsSuccess = await _historySourceCheckService.ExistsAsync(filter, Source.KeyAutoProbeg);
            if (existsSuccess)
            {
                var newCars = await _carService.GetNewCarsAndSaveAsync(carList, filter);
                cars.AddRange(newCars);
            }
            else
            {
                await _carService.GetNewCarsAndSaveAsync(carList, filter);
                await _historySourceCheckService.AddSourceAsync(filter, Source.KeyAutoProbeg);
                
                var existsAny = await _historySourceCheckService.ExistsAsync(filter);
                if (!existsAny)
                {
                    await _messageService.SendUserConfirmationMessageAsync(botClient, user);   
                }
            }
        }
        catch (Exception e)
        {
            _logger.LogWarning(
                $"User Filter ID: {filter.Id}. " +
                $"Method GetCarsFromKeyAutoProbegAsync: {e.Message}");
        }
    }
    
    private async Task GetCarsFromTradeDealerAsync(
        UserFilter filter, 
        List<Car> cars, 
        TelegramBotClient botClient, 
        AppUser user)
    {
        try
        {
            var tradeDealerFilter = _mapper.Map<TradeDealerFilter>(filter);
            var tradeDealerResult = await _tradeDealerHttpApiClient.GetAutoByFilterAsync(tradeDealerFilter);
            var carList = _mapper.Map<List<Car>>(tradeDealerResult.CarInfos);
            
            var existsSuccess = await _historySourceCheckService.ExistsAsync(filter, Source.TradeDealer);
            if (existsSuccess)
            {
                var newCars = await _carService.GetNewCarsAndSaveAsync(carList, filter);
                cars.AddRange(newCars);
            }
            else
            {
                await _carService.GetNewCarsAndSaveAsync(carList, filter);
                await _historySourceCheckService.AddSourceAsync(filter, Source.TradeDealer);
                
                var existsAny = await _historySourceCheckService.ExistsAsync(filter);
                if (!existsAny)
                {
                    await _messageService.SendUserConfirmationMessageAsync(botClient, user);   
                }
            }
        }
        catch (Exception e)
        {
            _logger.LogWarning(
                $"User Filter ID: {filter.Id}. " +
                $"Method GetCarsFromTradeDealerAsync: {e.Message}");
        }
    }
    
    private async Task GetCarsFromAutoRuAsync(
        UserFilter filter, 
        List<Car> cars, 
        TelegramBotClient botClient, 
        AppUser user)
    {
        try
        {
            var autoRuFilter = _mapper.Map<AutoRuFilter>(filter);
            var autoRuResult = await _autoRuHttpApiClient.GetAutoByFilterAsync(autoRuFilter);
            var carList = _mapper.Map<List<Car>>(autoRuResult.Offers);

            var existsSuccess = await _historySourceCheckService.ExistsAsync(filter, Source.AutoRu);
            if (existsSuccess)
            {
                var newCars = await _carService.GetNewCarsAndSaveAsync(carList, filter);
                cars.AddRange(newCars);
            }
            else
            {
                await _carService.GetNewCarsAndSaveAsync(carList, filter);
                await _historySourceCheckService.AddSourceAsync(filter, Source.AutoRu);
                
                var existsAny = await _historySourceCheckService.ExistsAsync(filter);
                if (!existsAny)
                {
                    await _messageService.SendUserConfirmationMessageAsync(botClient, user);   
                }
            }
        }
        catch (Exception e)
        {
            _logger.LogWarning(
                $"User Filter ID: {filter.Id}. " +
                $"Method GetCarsFromAutoRuAsync: {e.Message}");
        }
    }
}