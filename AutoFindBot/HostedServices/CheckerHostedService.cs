using AutoFindBot.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AutoFindBot.HostedServices;

public class CheckerHostedService : IHostedService, IDisposable
{
    private int executionCount = 0;
    private readonly ILogger<CheckerHostedService> _logger;
    private Timer? _timer = null;
    private readonly IAppUserService _appUserService;
    private readonly ICheckerNewAutoService _checkerNewAutoService;
    private readonly IMessageService _messageService;
    private int count;

    public CheckerHostedService(ILogger<CheckerHostedService> logger)
    {
        _logger = logger;
        _appUserService = ServiceLocator.GetService<IAppUserService>();
        _messageService = ServiceLocator.GetService<IMessageService>();
        _checkerNewAutoService = ServiceLocator.GetService<ICheckerNewAutoService>();
    }

    public Task StartAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation($"{nameof(CheckerHostedService)} running.");

        _timer = new Timer(DoWork, null, TimeSpan.Zero,
            TimeSpan.FromSeconds(5)); //900 - 15min

        return Task.CompletedTask;
    }

    private async void DoWork(object? state)
    {
        try
        {
            var users = await _appUserService.GetAllAsync();
            foreach (var user in users)
            {
                var newAutoList = await _checkerNewAutoService.GetNewAutoAsync(user);
                if (newAutoList.CarInfos.Any())
                {
                    //if()
                }
                
                count = Interlocked.Increment(ref executionCount);
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