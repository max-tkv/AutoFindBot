using Microsoft.Extensions.Logging;
using AutoFindBot.Abstractions;
using AutoFindBot.Entities;
using AutoFindBot.Exceptions;
using AutoFindBot.Helpers;
using AutoFindBot.Lookups;
using AutoFindBot.Utils.Helpers;
using Telegram.Bot.Types;

namespace AutoFindBot.Commands.Administration;

public class CheckRequiredSubscriptionsCommand : BaseCommand
{
    private readonly IAppUserService _appUserService;
    private readonly IMessageService _messageService;
    private readonly ILogger<CheckRequiredSubscriptionsCommand> _logger;
    public override string Name => CommandNames.CheckRequiredSubscriptionsCommand;

    public CheckRequiredSubscriptionsCommand(IAppUserService appUserService, IMessageService messageService,
        ILogger<CheckRequiredSubscriptionsCommand> logger)
    {
        _logger = logger;
        _appUserService = appUserService;
        _messageService = messageService;
    }
    
    public override async Task ExecuteAsync(Update update, AppUser user)
    {
        try
        {
            await _appUserService.CheckRequiredSubscriptionsAsync(user);
            await _messageService.SendStartMessage(user);
        }
        catch (RequiredSubscriptionsException)
        {
            try
            {
                await _messageService.SendPopupMessageAsync(user, update, 
                    Messages.RequiredSubscriptionsPopupError.GetDescription());
            }
            catch (Exception e)
            {
                _logger.LogError($"UserID: {user.Id} CheckRequiredSubscriptionsAsync() Error: {e.Message} Trace: {e.StackTrace}");
            }
        }
        catch (Exception e)
        {
            await _messageService.SendErrorMessageAsync(user, CommandHelpers.GetErrorMessage(Name, update.Message.Text));
            _logger.LogError($"UserID: {user.Id} CommandName: {Name} Error: {e.Message} Trace: {e.StackTrace}");
        }
    }
}