using Ardalis.GuardClauses;
using AutoFindBot.Abstractions;
using AutoFindBot.Abstractions.HttpClients;
using AutoFindBot.Entities;
using AutoFindBot.Exceptions;
using AutoFindBot.Lookups;
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
    private readonly IYoulaHttpApiClient _youlaHttpApiClient;

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
        IDromHttpApiClient dromHttpApiClient,
        IYoulaHttpApiClient youlaHttpApiClient)
    {
        _logger = Guard.Against.Null(logger, nameof(logger));
        _keyAutoProbegHttpApiClient = Guard.Against.Null(keyAutoProbegHttpApiClient, nameof(keyAutoProbegHttpApiClient));
        _tradeDealerHttpApiClient = Guard.Against.Null(tradeDealerHttpApiClient, nameof(tradeDealerHttpApiClient));
        _autoRuHttpApiClient = Guard.Against.Null(autoRuHttpApiClient, nameof(autoRuHttpApiClient));
        _avitoHttpApiClient = Guard.Against.Null(avitoHttpApiClient, nameof(avitoHttpApiClient));
        _userFilterRepository = Guard.Against.Null(userFilterRepository, nameof(userFilterRepository));
        _messageService = Guard.Against.Null(messageService, nameof(messageService));
        _carService = Guard.Against.Null(carService, nameof(carService));
        _mapper = Guard.Against.Null(mapper, nameof(mapper));
        _appUserService = Guard.Against.Null(appUserService, nameof(appUserService));
        _sourceCheckRepository = Guard.Against.Null(sourceCheckRepository, nameof(sourceCheckRepository));
        _dromHttpApiClient = Guard.Against.Null(dromHttpApiClient, nameof(dromHttpApiClient));
        _youlaHttpApiClient = Guard.Against.Null(youlaHttpApiClient, nameof(youlaHttpApiClient));
    }

    public async Task CheckAndSendMessageAsync(
        TelegramBotClient botClient, 
        AppUser? user = null, 
        CancellationToken stoppingToken = default)
    {
        var newCars = await GetNewAutoAsync(stoppingToken);
        if (!newCars.Any())
        {
            _logger.LogInformation("New auto not found");
            return;
        }

        if (user != null)
        {
            await CheckAutoAndSendMessageByUserAsync(
                botClient, user, newCars, true, stoppingToken);
            return;
        }
        
        var users = await _appUserService.GetAllAsync(stoppingToken);
        foreach (var currentUser in users)
        {
            await CheckAutoAndSendMessageByUserAsync(
                botClient, currentUser, newCars, false, stoppingToken);
        }
    }

    private async Task CheckAutoAndSendMessageByUserAsync(
        TelegramBotClient botClient, 
        AppUser currentUser, 
        List<Car> newCars, 
        bool sendEmptyResultMessage = false, 
        CancellationToken stoppingToken = default)
    {
        var filters = await _userFilterRepository.GetByUserAsync(currentUser, stoppingToken);
        _logger.LogInformation($"User ID: {currentUser.Id}. Find {filters.Count} filters.");
        foreach (var currentFilter in filters)
        {
            _logger.LogInformation($"User ID: {currentUser.Id}. Select Filter ID: {currentFilter.Id}");
            var isFoundNewAutoByFilter = false;
            var newCarsGroupsBySource = newCars.GroupBy(x => x.SourceType);
            foreach (var newCarsGroupBySource in newCarsGroupsBySource)
            {
                var currentSource = newCarsGroupBySource.Key;
                var newCarsSource = newCarsGroupBySource.ToList();
                var checkedNewCars = await CheckAutoAsync(
                    newCarsSource, currentFilter, currentSource, stoppingToken);
                if (checkedNewCars.Any())
                {
                    var existsSuccess = await _sourceCheckRepository.ExistsAsync(
                        currentFilter, currentSource, stoppingToken);
                    if (existsSuccess)
                    {
                        await _messageService.SendNewAutoMessageAsync(
                            botClient, currentUser, currentFilter, checkedNewCars, stoppingToken);
                        isFoundNewAutoByFilter = true;
                    }
                    else
                    {
                        await _sourceCheckRepository.AddAsync(new SourceCheck()
                        {
                            SourceType = currentSource,
                            UserFilterId = currentFilter.Id
                        }, stoppingToken);
                        if (!currentUser.Confirm)
                        {
                            await _messageService.SendUserConfirmationMessageAsync(
                                botClient, currentUser, stoppingToken);
                            await _appUserService.SetConfirmAsync(
                                currentUser.Id, stoppingToken);
                            currentUser.Confirm = true;
                        }
                    }
                }
            }
            
            if (!isFoundNewAutoByFilter && sendEmptyResultMessage)                                                           
            {                                                                                                        
                await _messageService.SendNewAutoMessageAsync(
                    botClient, currentUser, currentFilter, new List<Car>(), stoppingToken);
            }                                                                                                           
        }
    }

    private async Task<List<Car>> CheckAutoAsync(
        List<Car> newCars, 
        UserFilter currentFilter, 
        SourceType sourceType, 
        CancellationToken stoppingToken = default)
    {
        var result = new List<Car>();
        foreach (var newCar in newCars)
        {
            var filterSuccess = CheckAutoByUserFilter(currentFilter, newCar);
            if (filterSuccess)
            {
                var checkedNewCar = await _carService.CheckExistNewCarAndSaveAsync(
                    newCar, currentFilter, sourceType, stoppingToken);
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
   

    private async Task<List<Car>> GetNewAutoAsync(CancellationToken stoppingToken = default)
    {
        var cars = new List<Car>();

        await GetCarsFromAutoRuAsync(cars, stoppingToken);
        await GetCarsFromTradeDealerAsync(cars, stoppingToken);
        await GetCarsFromKeyAutoProbegAsync(cars, stoppingToken);
        await GetCarsFromAvitoAsync(cars, stoppingToken);
        await GetCarsFromDromAsync(cars, stoppingToken);
        await GetCarsFromYoulaAsync(cars, stoppingToken);

        return cars;
    }

    private async Task GetCarsFromDromAsync(
        List<Car> cars, 
        CancellationToken stoppingToken = default)
    {
        try
        {
            var avitoResult = await _dromHttpApiClient.GetAllNewAutoAsync(stoppingToken);
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

    private async Task GetCarsFromAvitoAsync(
        List<Car> cars, 
        CancellationToken stoppingToken = default)
    {
        try
        {
            var avitoResult = await _avitoHttpApiClient.GetAllNewAutoAsync(stoppingToken);
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
    
    private async Task GetCarsFromKeyAutoProbegAsync(
        List<Car> cars, 
        CancellationToken stoppingToken = default)
    {
        try
        {
            var keyAutoProbegResult = await _keyAutoProbegHttpApiClient.GetAllNewAutoAsync(stoppingToken);
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
    
    private async Task GetCarsFromTradeDealerAsync(
        List<Car> cars, 
        CancellationToken stoppingToken = default)
    {
        try
        {
            var tradeDealerResult = await _tradeDealerHttpApiClient.GetAllNewAutoAsync(stoppingToken);
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
    
    private async Task GetCarsFromAutoRuAsync(
        List<Car> cars, 
        CancellationToken stoppingToken = default)
    {
        try
        {
            var autoRuResult = await _autoRuHttpApiClient.GetAllNewAutoAsync(stoppingToken);
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
                $"Method GetCarsFromAutoRuAsync: {e}");
        }
    }
    
    private async Task GetCarsFromYoulaAsync(
        List<Car> cars, 
        CancellationToken stoppingToken = default)
    {
        try
        {
            var autoRuResult = await _youlaHttpApiClient.GetAllNewAutoAsync(stoppingToken);
            var newCars = _mapper.Map<List<Car>>(autoRuResult.Cars);
            cars.AddRange(newCars);
            
            _logger.LogInformation($"Request by GetCarsFromYoulaAsync is success. " +
                                   $"Found: {newCars.Count}");
        }
        catch (NotActiveSourceException e)
        {
            _logger.LogWarning(e.Message);
        }
        catch (Exception e)
        {
            _logger.LogWarning(
                $"Method GetCarsFromYoulaAsync: {e}");
        }
    }
}