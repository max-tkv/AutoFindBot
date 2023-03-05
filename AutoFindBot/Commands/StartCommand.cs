using Microsoft.Extensions.Logging;
using AutoFindBot.Abstractions;
using AutoFindBot.Entities;
using AutoFindBot.Helpers;
using Telegram.Bot.Types;

namespace AutoFindBot.Commands
{
    public class StartCommand : BaseCommand
    {
        private readonly IAppUserService _appUserService;
        private readonly IActionService _actionService;
        private readonly ILogger<StartCommand> _logger;
        private readonly IMessageService _messageService;

        public override string Name => CommandNames.StartCommand;
        
        public StartCommand(IAppUserService appUserService, IActionService actionService, 
            ILogger<StartCommand> logger, IMessageService messageService)
        {
            _appUserService = appUserService;
            _actionService = actionService;
            _logger = logger;
            _messageService = messageService;
        }

        public override async Task ExecuteAsync(Update update, AppUser user)
        {
            try
            {
                if (user.ChatId != 983077680)
                {
                    await _messageService.SendErrorMessageAsync(user, "Sorry. Bot is in development.");
                }
                else
                {
                    await _messageService.SendStartMessage(user);   
                }
                await _actionService.AddAsync(new Entities.Action()
                {
                    UserId = user.Id,
                    CommandName = Name
                });
            }
            catch (Exception e)
            {
                await _messageService.SendErrorMessageAsync(user, CommandHelpers.GetErrorMessage(Name));
                _logger.LogError(e, $"UserID: {user.Id} CommandName: {Name} Error: {e.Message} Trace: {e.StackTrace}");
            }
        }
    }
}