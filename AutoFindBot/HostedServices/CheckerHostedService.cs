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
    private readonly ICheckingNewAutoService _checkingNewAutoService;
    private readonly TelegramBotClient _botClient;

    public CheckerHostedService(ILogger<CheckerHostedService> logger)
    {
        _logger = logger;
        _appUserService = ServiceLocator.GetService<IAppUserService>();
        _checkingNewAutoService = ServiceLocator.GetService<ICheckingNewAutoService>();
        
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
            _logger.LogInformation($"{nameof(CheckerHostedService)}: Start DoWork method.");
            
            var users = await _appUserService.GetAllAsync();
            foreach (var user in users)
            {
                _logger.LogInformation($"Select User ID: {user.Id}");
                await _checkingNewAutoService.CheckAndSendMessageAsync(_botClient, user);
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