using AutoFindBot.Abstractions;
using AutoFindBot.Entities;
using AutoFindBot.Services;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace AutoFindBot.Commands;

public class CheckNewAutoCommand : BaseCommand
{
    private readonly ICheckingNewAutoService _checkingNewAutoService;
    private readonly ILogger<StartCommand> _logger;
    private readonly TelegramBotClient _botClient;

    public override string Name => CommandNames.CheckNewAutoCommand;

    public CheckNewAutoCommand(
        ICheckingNewAutoService checkingNewAutoService,
        ILogger<StartCommand> logger,
        TelegramBotService telegramBotService)
    {
        _checkingNewAutoService = checkingNewAutoService;
        _botClient = telegramBotService.GetBotAsync().Result;
        _logger = logger;
    }
    
    public override async Task ExecuteAsync(
        Update update, 
        AppUser user, 
        CancellationToken stoppingToken = default)
    {
        try
        {
            _logger.LogInformation($"Current User ID: {user.Id}");
            await _checkingNewAutoService.CheckAndSendMessageAsync(_botClient, user, stoppingToken);
        }
        catch (Exception e)
        {
            _logger.LogError($"User ID: {user.Id}. Error: {e.Message}");
        }
    }
}