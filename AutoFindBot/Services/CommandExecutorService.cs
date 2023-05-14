using Microsoft.Extensions.DependencyInjection;
using AutoFindBot.Abstractions;
using Telegram.Bot.Types;
using AutoFindBot.Commands;
using AutoFindBot.Entities;

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
        
        public async Task ExecuteAsync(
            Update update, 
            string? commandName = null, 
            CancellationToken stoppingToken = default)
        {
            var appUser = _appUserService.GetByUpdate(update);
            var user = await _appUserService.GetOrCreateAsync(appUser, stoppingToken);
            await RunAsync(update, user, commandName, stoppingToken);
        }

        private async Task RunAsync(
            Update update,
            AppUser user, 
            string? commandName = null, 
            CancellationToken stoppingToken = default)
        {
            if (commandName != null)
            {
                await ExecuteCommandAsync(commandName, update, user, stoppingToken);
                return;
            }

            if (update.PreCheckoutQuery != null)
            {
                await _paymentService.SavePaymentAsync(update, user, stoppingToken);
                return;
            }
            
            if(update.Message?.Chat == null && update?.CallbackQuery == null)
                return;

            if (update.Message != null)
            {
                if (update.Message.Text.StartsWith(CommandNames.StartCommand))
                {
                    await ExecuteCommandAsync(CommandNames.StartCommand, update, user, stoppingToken);
                    return;
                }
                
                if (update.Message.Text.StartsWith(CommandNames.CheckNewAutoCommand))
                {
                    await ExecuteCommandAsync(CommandNames.CheckNewAutoCommand, update, user, stoppingToken);
                    return;
                }
                
                if (update.Message.Text.StartsWith(CommandNames.SettingsCommand))
                {
                    await ExecuteCommandAsync(CommandNames.SettingsCommand, update, user, stoppingToken);
                    return;
                }
            }
            
            if (update.CallbackQuery?.Data is CommandNames.FiltersCommand)
            {
                await ExecuteCommandAsync(CommandNames.FiltersCommand, update, user, stoppingToken);
                return;
            }
            
            if (update.CallbackQuery?.Data is CommandNames.SourcesCommand)
            {
                await ExecuteCommandAsync(CommandNames.SourcesCommand, update, user, stoppingToken);
                return;
            }
            
            var lastAction = await _actionService.GetLastByUserAsync(user, stoppingToken);
            switch ((lastAction?.CommandName, lastAction?.Category))
            {
                // case (CommandNames.BackCommand, Categories.Films):
                // {
                //     await ExecuteCommand(CommandNames.FindFilmsCommand, update, user);
                //     break;
                // }
            }
        }

        private async Task ExecuteCommandAsync(
            string commandName, 
            Update update, 
            AppUser user, 
            CancellationToken stoppingToken = default)
        {
           await _commands.First(x => x.Name == commandName)
               .ExecuteAsync(update, user, stoppingToken);
        }
    }
}