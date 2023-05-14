using Microsoft.Extensions.Logging;
using AutoFindBot.Abstractions;
using AutoFindBot.Entities;
using AutoFindBot.Helpers;
using AutoFindBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace AutoFindBot.Commands
{
    public class StartCommand : BaseCommand
    {
        private readonly IActionService _actionService;
        private readonly ILogger<StartCommand> _logger;
        private readonly IMessageService _messageService;
        private readonly TelegramBotClient _botClient;

        public override string Name => CommandNames.StartCommand;
        
        public StartCommand(
            IActionService actionService, 
            ILogger<StartCommand> logger, 
            IMessageService messageService,
            TelegramBotService telegramBotService)
        {
            _actionService = actionService;
            _logger = logger;
            _messageService = messageService;
            _botClient = telegramBotService.GetBotAsync().Result;
        }

        public override async Task ExecuteAsync(
            Update update, 
            AppUser user, 
            CancellationToken stoppingToken = default)
        {
            try
            {
                await _messageService.SendStartMessageAsync(_botClient, user, stoppingToken);
                await _actionService.AddAsync(new Entities.Action()
                {
                    UserId = user.Id,
                    CommandName = Name
                }, stoppingToken);
            }
            catch (Exception e)
            {
                await _messageService.SendErrorMessageAsync(
                    _botClient, user, CommandHelpers.GetErrorMessage(Name), stoppingToken);
                _logger.LogError($"UserID: {user.Id} CommandName: {Name} Error: {e.Message} Trace: {e.StackTrace}");
            }
        }
    }
}