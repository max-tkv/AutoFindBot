using Microsoft.Extensions.DependencyInjection;
using AutoFindBot.Abstractions;
using Telegram.Bot.Types;
using AutoFindBot.Commands;
using AutoFindBot.Entities;
using AutoFindBot.Extensions;
using Telegram.Bot;
using Emoji = AutoFindBot.Lookups.Emoji;

namespace AutoFindBot.Services
{
    public class CommandExecutorService : ICommandExecutorService
    {
        private readonly List<BaseCommand> _commands;
        private readonly IActionService _actionService;
        private readonly IAppUserService _appUserService;
        private readonly IPaymentService _paymentService;

        public CommandExecutorService(IServiceProvider serviceProvider, IActionService actionService,
            IAppUserService appUserService, IPaymentService paymentService)
        {
            _commands = serviceProvider.GetServices<BaseCommand>().ToList();
            _actionService = actionService;
            _appUserService = appUserService;
            _paymentService = paymentService;
        }
        
        public async Task ExecuteAsync(Update update, string? commandName = null)
        {
            var appUser = _appUserService.GetByUpdate(update);
            var user = await _appUserService.GetOrCreateAsync(appUser);
            await RunAsync(update, user, commandName);
        }

        private async Task RunAsync(Update update, AppUser user, string? commandName = null)
        {
            if (commandName != null)
            {
                await ExecuteCommand(commandName, update, user);
                return;
            }

            if (update.PreCheckoutQuery != null)
            {
                await _paymentService.SavePaymentAsync(update, user);
                return;
            }
            
            if(update.Message?.Chat == null && update?.CallbackQuery == null)
                return;
            
            // if (update.CallbackQuery?.Data is CommandNames.DownloadTrackCommand)
            // {
            //     await ExecuteCommand(update.CallbackQuery?.Data, update, user);
            //     return;
            // }

            if (update.Message != null)
            {
                if (update.Message.Text.StartsWith(CommandNames.StartCommand))
                {
                    await ExecuteCommand(CommandNames.StartCommand, update, user);
                    return;
                }
                
                if (update.Message.Text.StartsWith(CommandNames.CheckNewAutoCommand))
                {
                    await ExecuteCommand(CommandNames.CheckNewAutoCommand, update, user);
                    return;
                }
                
                if (update.Message.Text.StartsWith(CommandNames.SettingsCommand))
                {
                    await ExecuteCommand(CommandNames.SettingsCommand, update, user);
                    return;
                }
            }
            
            var lastAction = await _actionService.GetLastByUserAsync(user);
            switch ((lastAction.CommandName, lastAction.Category))
            {
                // case (CommandNames.BackCommand, Categories.Films):
                // {
                //     await ExecuteCommand(CommandNames.FindFilmsCommand, update, user);
                //     break;
                // }
            }
        }

        private async Task ExecuteCommand(string commandName, Update update, AppUser user)
        {
           await _commands.First(x => x.Name == commandName)
               .ExecuteAsync(update, user);
        }
    }
}