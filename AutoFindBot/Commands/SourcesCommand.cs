using AutoFindBot.Abstractions;
using AutoFindBot.Entities;
using AutoFindBot.Services;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace AutoFindBot.Commands;

public class SourcesCommand : BaseCommand
{
    private readonly ILogger<SourcesCommand> _logger;
    private readonly TelegramBotClient _botClient;
    private readonly IMessageService _messageService;

    public override string Name => CommandNames.SourcesCommand;

    public SourcesCommand(
        IMessageService messageService,
        ILogger<SourcesCommand> logger,
        TelegramBot telegramBot)
    {
        _botClient = telegramBot.GetBot().Result;
        _messageService = messageService;
        _logger = logger;
    }
    
    public override async Task ExecuteAsync(Update update, AppUser user)
    {
        await _messageService.SendSettingsCommands(_botClient, update, user);
    }
}