using AutoFindBot.Abstractions;
using AutoFindBot.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace AutoFindBot.HostedServices;

public class CheckerHostedService : IHostedService, IDisposable
{
    private readonly ILogger<CheckerHostedService> _logger;
    private Timer? _timer;
    private readonly IAppUserService _appUserService;
    private readonly ICheckerNewAutoService _checkerNewAutoService;
    private readonly IMessageService _messageService;
    private readonly IUserFilterService _userFilterService;
    private readonly TelegramBotClient _botClient;
    private readonly ICarService _carService;

    public CheckerHostedService(ILogger<CheckerHostedService> logger)
    {
        _logger = logger;
        _appUserService = ServiceLocator.GetService<IAppUserService>();
        _messageService = ServiceLocator.GetService<IMessageService>();
        _checkerNewAutoService = ServiceLocator.GetService<ICheckerNewAutoService>();
        _userFilterService = ServiceLocator.GetService<IUserFilterService>();
        _carService = ServiceLocator.GetService<ICarService>();
        
        var telegramBot = ServiceLocator.GetService<TelegramBot>();
        _botClient = telegramBot.GetBot().Result;
    }

    public Task StartAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation($"{nameof(CheckerHostedService)} running.");

        _timer = new Timer(DoWork, null, TimeSpan.Zero,
            TimeSpan.FromSeconds(60)); //900 - 15min

        return Task.CompletedTask;
    }

    private async void DoWork(object? state)
    {
        try
        {
            var users = await _appUserService.GetAllAsync();
            foreach (var user in users)
            {
                _logger.LogInformation($"Select User ID: {user.Id}");
                
                var filters = await _userFilterService.GetByUserAsync(user);
                foreach (var filter in filters)
                {
                    _logger.LogInformation($"User ID: {user.Id}. Select Filter ID: {filter.Id}");
                    
                    var newAutoList = await _checkerNewAutoService.GetAutoByFilterAsync(user, filter);
                    var newCars = await _carService.GetNewCarsAndSaveAsync(newAutoList.CarInfos, user, filter);
                    if (newCars.Any())
                    {
                        await _messageService.SendNewAutoMessageAsync(_botClient, user, filter, newCars);   
                    }
                    _logger.LogInformation($"Find new cars: {newCars.Count}");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error to {nameof(CheckerHostedService)}: {ex.Message}");
        }
    }

    public Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation($"{nameof(CheckerHostedService)} is stopping.");

        _timer?.Change(Timeout.Infinite, 0);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}