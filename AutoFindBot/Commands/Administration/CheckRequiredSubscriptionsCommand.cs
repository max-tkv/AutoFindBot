using Microsoft.Extensions.Logging;
using AutoFindBot.Abstractions;
using AutoFindBot.Entities;
using AutoFindBot.Exceptions;
using AutoFindBot.Helpers;
using AutoFindBot.Invariants;
using AutoFindBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace AutoFindBot.Commands.Administration;

public class CheckRequiredSubscriptionsCommand : BaseCommand
{
    private readonly IAppUserService _appUserService;
    private readonly IMessageService _messageService;
    private readonly ILogger<CheckRequiredSubscriptionsCommand> _logger;
    private readonly TelegramBotClient _botClient;
    
    public override string Name => CommandNames.CheckRequiredSubscriptionsCommand;

    public CheckRequiredSubscriptionsCommand(
        IAppUserService appUserService, 
        IMessageService messageService,
        ILogger<CheckRequiredSubscriptionsCommand> logger,
        TelegramBot telegramBot)
    {
        _logger = logger;
        _appUserService = appUserService;
        _messageService = messageService;
        _botClient = telegramBot.GetBot().Result;
    }
    
    public override async Task ExecuteAsync(Update update, AppUser user)
    {
        try
        {
            await _appUserService.CheckRequiredSubscriptionsAsync(_botClient, user);
            await _messageService.SendStartMessage(_botClient, user);
        }
        catch (RequiredSubscriptionsException)
        {
            try
            {
                await _messageService.SendPopupMessageAsync(_botClient, user, update, 
                    Messages.RequiredSubscriptionsPopupError);
            }
            catch (Exception e)
            {
                _logger.LogError($"UserID: {user.Id} CheckRequiredSubscriptionsAsync() Error: {e.Message} Trace: {e.StackTrace}");
            }
        }
        catch (Exception e)
        {
            await _messageService.SendErrorMessageAsync(_botClient, user, CommandHelpers.GetErrorMessage(Name, update.Message.Text));
            _logger.LogError($"UserID: {user.Id} CommandName: {Name} Error: {e.Message} Trace: {e.StackTrace}");
        }
    }
}