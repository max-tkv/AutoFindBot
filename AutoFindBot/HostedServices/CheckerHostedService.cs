using AutoFindBot.Abstractions;
using AutoFindBot.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace AutoFindBot.HostedServices;

public class CheckerHostedService : IHostedService, IDisposable
{
    private readonly ILogger<CheckerHostedService> _logger;
    private Timer? _timer;
    private readonly ICheckingNewAutoService _checkingNewAutoService;
    private readonly TelegramBotClient _botClient;
    private readonly IConfiguration _configuration;

    public CheckerHostedService(ILogger<CheckerHostedService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
        _checkingNewAutoService = ServiceLocator.GetService<ICheckingNewAutoService>();
        
        var telegramBot = ServiceLocator.GetService<TelegramBotService>();
        _botClient = telegramBot.GetBotAsync().Result;
    }

    public Task StartAsync(CancellationToken stoppingToken)
    {
        var checkTimer = _configuration.GetValue<int>("CheckTimer");
        _timer = new Timer(
            DoWorkAsync, 
            stoppingToken, 
            TimeSpan.Zero,
            TimeSpan.FromSeconds(checkTimer));

        return Task.CompletedTask;
    }

    private async void DoWorkAsync(object? stoppingToken)
    {
        try
        {
            _logger.LogInformation($"Start DoWork method.");

            await _checkingNewAutoService.CheckAndSendMessageAsync(_botClient, null, (CancellationToken)stoppingToken!);
            
            _logger.LogInformation($"End DoWork method.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
        }
    }

    public Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation($"Is stopping.");

        _timer?.Change(Timeout.Infinite, 0);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}