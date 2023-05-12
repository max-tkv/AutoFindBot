using AutoFindBot.Abstractions;
using AutoFindBot.Abstractions.HttpClients;
using AutoFindBot.Entities;
using AutoFindBot.Exceptions;
using AutoFindBot.Repositories;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace AutoFindBot.Services;

public class CheckingNewAutoService : ICheckingNewAutoService
{
    private readonly ILogger<CheckingNewAutoService> _logger;
    private readonly ITradeDealerHttpApiClient _tradeDealerHttpApiClient;
    private readonly IUserFilterRepository _userFilterRepository;
    private readonly IMessageService _messageService;
    private readonly ICarService _carService;
    private readonly IMapper _mapper;
    private readonly IKeyAutoProbegHttpApiClient _keyAutoProbegHttpApiClient;
    private readonly IAvitoHttpApiClient _avitoHttpApiClient;
    private readonly IAutoRuHttpApiClient _autoRuHttpApiClient;
    private readonly IAppUserService _appUserService;
    private readonly ISourceCheckRepository _sourceCheckRepository;
    private readonly IDromHttpApiClient _dromHttpApiClient;

    public CheckingNewAutoService(
        ILogger<CheckingNewAutoService> logger,
        IKeyAutoProbegHttpApiClient keyAutoProbegHttpApiClient,
        ITradeDealerHttpApiClient tradeDealerHttpApiClient,
        IAutoRuHttpApiClient autoRuHttpApiClient,
        IAvitoHttpApiClient avitoHttpApiClient,
        IUserFilterRepository userFilterRepository,
        IMessageService messageService,
        ICarService carService,
        IMapper mapper,
        IAppUserService appUserService,
        ISourceCheckRepository sourceCheckRepository,
        IDromHttpApiClient dromHttpApiClient)
    {
        _logger = logger;
        _keyAutoProbegHttpApiClient = keyAutoProbegHttpApiClient;
        _tradeDealerHttpApiClient = tradeDealerHttpApiClient;
        _autoRuHttpApiClient = autoRuHttpApiClient;
        _avitoHttpApiClient = avitoHttpApiClient;
        _userFilterRepository = userFilterRepository;
        _messageService = messageService;
        _carService = carService;
        _mapper = mapper;
        _appUserService = appUserService;
        _sourceCheckRepository = sourceCheckRepository;
        _dromHttpApiClient = dromHttpApiClient;
    }

    public async Task CheckAndSendMessageAsync(TelegramBotClient botClient, AppUser? user = null)
    {
        var newCars = await GetNewAutoAsync();
        if (!newCars.Any())
        {
            _logger.LogInformation("New auto not found");
            return;
        }

        if (user != null)
        {
            await CheckAutoAndSendMessageByUserAsync(botClient, user, newCars, true);
            return;
        }
        
        var users = await _appUserService.GetAllAsync();
        foreach (var currentUser in users)
        {
            await CheckAutoAndSendMessageByUserAsync(botClient, currentUser, newCars);
        }
    }

    private async Task CheckAutoAndSendMessageByUserAsync(
        TelegramBotClient botClient, AppUser currentUser, List<Car> newCars, bool sendEmptyResultMessage = false)
    {
        var filters = await _userFilterRepository.GetByUserAsync(currentUser);
        _logger.LogInformation($"User ID: {currentUser.Id}. Find {filters.Count} filters.");
        foreach (var currentFilter in filters)
        {
            _logger.LogInformation($"User ID: {currentUser.Id}. Select Filter ID: {currentFilter.Id}");
            var isFoundNewAutoByFilter = false;
            var newCarsGroupsBySource = newCars.GroupBy(x => x.Source);
            foreach (var newCarsGroupBySource in newCarsGroupsBySource)
            {
                var currentSource = newCarsGroupBySource.Key;
                var newCarsSource = newCarsGroupBySource.ToList();
                var checkedNewCars = await CheckAutoAsync(newCarsSource, currentFilter, currentSource);
                if (checkedNewCars.Any())
                {
                    var existsSuccess = await _sourceCheckRepository.ExistsAsync(currentFilter, currentSource);
                    if (existsSuccess)
                    {
                        await _messageService.SendNewAutoMessageAsync(botClient, currentUser, currentFilter, checkedNewCars);
                        isFoundNewAutoByFilter = true;
                    }
                    else
                    {
                        await _sourceCheckRepository.AddAsync(new SourceCheck()
                        {
                            Source = currentSource,
                            UserFilterId = currentFilter.Id
                        });
                        if (!currentUser.Confirm)
                        {
                            await _messageService.SendUserConfirmationMessageAsync(botClient, currentUser);
                            await _appUserService.SetConfirmAsync(currentUser.Id);
                            currentUser.Confirm = true;
                        }
                    }
                }
            }
            
            if (!isFoundNewAutoByFilter && sendEmptyResultMessage)                                                           
            {                                                                                                        
                await _messageService.SendNewAutoMessageAsync(botClient, currentUser, currentFilter, new List<Car>());
            }                                                                                                           
        }
    }

    private async Task<List<Car>> CheckAutoAsync(List<Car> newCars, UserFilter currentFilter, Source source)
    {
        var result = new List<Car>();
        foreach (var newCar in newCars)
        {
            var filterSuccess = CheckAutoByUserFilter(currentFilter, newCar);
            if (filterSuccess)
            {
                var checkedNewCar = await _carService.CheckExistNewCarAndSaveAsync(newCar, currentFilter, source);
                if (checkedNewCar)
                {
                    result.Add(newCar);
                }
            }
        }

        return result;
    }

    private bool CheckAutoByUserFilter(UserFilter currentFilter, Car newCar)
    {
        if (newCar.Price < currentFilter.PriceMin || newCar.Price > currentFilter.PriceMax)
        {
            return false;
        }
        
        if (newCar.Year < currentFilter.YearMin || newCar.Year > currentFilter.YearMax)
        {
            return false;
        }
        
        return true;
    }
   

    private async Task<List<Car>> GetNewAutoAsync()
    {
        var cars = new List<Car>();

        await GetCarsFromAutoRuAsync(cars);
        await GetCarsFromTradeDealerAsync(cars);
        await GetCarsFromKeyAutoProbegAsync(cars);
        await GetCarsFromAvitoAsync(cars);
        await GetCarsFromDromAsync(cars);

        return cars;
    }

    private async Task GetCarsFromDromAsync(List<Car> cars)
    {
        try
        {
            var avitoResult = await _dromHttpApiClient.GetAllNewAutoAsync();
            var newCars = _mapper.Map<List<Car>>(avitoResult);
            cars.AddRange(newCars);
            
            _logger.LogInformation($"Request by GetCarsFromDromAsync is success. " +
                                   $"Found: {newCars.Count}");
        }
        catch (NotActiveSourceException e)
        {
            _logger.LogWarning(e.Message);
        }
        catch (Exception e)
        {
            _logger.LogWarning(
                $"Method GetCarsFromDromAsync: {e.Message}");
        }
    }

    private async Task GetCarsFromAvitoAsync(List<Car> cars)
    {
        try
        {
            var avitoResult = await _avitoHttpApiClient.GetAllNewAutoAsync();
            var newCars = _mapper.Map<List<Car>>(avitoResult);
            cars.AddRange(newCars);
            
            _logger.LogInformation($"Request by GetCarsFromAvitoAsync is success. " +
                                   $"Found: {newCars.Count}");
        }
        catch (NotActiveSourceException e)
        {
            _logger.LogWarning(e.Message);
        }
        catch (Exception e)
        {
            _logger.LogWarning(
                $"Method GetCarsFromAvitoAsync: {e.Message}");
        }
    }
    
    private async Task GetCarsFromKeyAutoProbegAsync(List<Car> cars)
    {
        try
        {
            var keyAutoProbegResult = await _keyAutoProbegHttpApiClient.GetAllNewAutoAsync();
            var newCars = _mapper.Map<List<Car>>(keyAutoProbegResult);
            cars.AddRange(newCars);
            
            _logger.LogInformation($"Request by GetCarsFromKeyAutoProbegAsync is success. " +
                                   $"Found: {newCars.Count}");
        }
        catch (NotActiveSourceException e)
        {
            _logger.LogWarning(e.Message);
        }
        catch (Exception e)
        {
            _logger.LogWarning(
                $"Method GetCarsFromKeyAutoProbegAsync: {e.Message}");
        }
    }
    
    private async Task GetCarsFromTradeDealerAsync(List<Car> cars)
    {
        try
        {
            var tradeDealerResult = await _tradeDealerHttpApiClient.GetAllNewAutoAsync();
            var newCars = _mapper.Map<List<Car>>(tradeDealerResult.CarInfos);
            cars.AddRange(newCars);
            
            _logger.LogInformation($"Request by GetCarsFromTradeDealerAsync is success. " +
                                   $"Found: {newCars.Count}");
        }
        catch (NotActiveSourceException e)
        {
            _logger.LogWarning(e.Message);
        }
        catch (Exception e)
        {
            _logger.LogWarning(
                $"Method GetCarsFromTradeDealerAsync: {e.Message}");
        }
    }
    
    private async Task GetCarsFromAutoRuAsync(List<Car> cars)
    {
        try
        {
            var autoRuResult = await _autoRuHttpApiClient.GetAllNewAutoAsync();
            var newCars = _mapper.Map<List<Car>>(autoRuResult.Offers);
            cars.AddRange(newCars);
            
            _logger.LogInformation($"Request by GetCarsFromAutoRuAsync is success. " +
                                   $"Found: {newCars.Count}");
        }
        catch (NotActiveSourceException e)
        {
            _logger.LogWarning(e.Message);
        }
        catch (Exception e)
        {
            _logger.LogWarning(
                $"Method GetCarsFromAutoRuAsync: {e.Message}");
        }
    }
}