using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AutoFindBot.Abstractions;
using AutoFindBot.Commands;
using AutoFindBot.Models.ConfigurationOptions;
using AutoFindBot.Services;

namespace AutoFindBot;

public static class Entry
{
    /// <summary>
    /// Регистрация зависимостей уровня бизнес-логики
    /// </summary>
    /// <param name="serviceCollection">serviceCollection</param>
    /// <returns>IServiceCollection</returns>
    public static IServiceCollection AddDomain(this IServiceCollection services)
    {
        // services
        services.AddSingleton<TelegramBot>();
        
        services.AddTransient<ICommandExecutorService, CommandExecutorService>();
        services.AddTransient<IKeyboardService, KeyboardService>();
        services.AddTransient<IMessageService, MessageService>();
        services.AddTransient<IAppUserService, AppUserService>();
        services.AddTransient<IActionService, ActionService>();
        services.AddTransient<IPaginationService, PaginationService>();
        services.AddTransient<IPaymentService, PaymentService>();
        services.AddTransient<ICheckerNewAutoService, CheckerNewAutoService>();
        services.AddTransient<IUserFilterService, UserFilterService>();

        // commands
        services.AddTransient<BaseCommand, StartCommand>();

        return services;
    }
    
    public static IServiceCollection AddConfig(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<PaymentsOptions>(configuration.GetSection(new PaymentsOptions().Name));
        services.Configure<RequiredSubscriptionsOptions>(configuration.GetSection(new RequiredSubscriptionsOptions().Name));
        
        return services;
    }
}