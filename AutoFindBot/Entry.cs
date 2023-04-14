using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AutoFindBot.Abstractions;
using AutoFindBot.Commands;
using AutoFindBot.HostedServices;
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
        services.AddTransient<ICheckingNewAutoService, CheckingNewAutoService>();
        services.AddTransient<IUserFilterService, UserFilterService>();
        services.AddTransient<ICarService, CarService>();
        services.AddTransient<ICaptchaSolutionsService, CaptchaSolutionsService>();

        // commands
        services.AddTransient<BaseCommand, StartCommand>();
        services.AddTransient<BaseCommand, CheckNewAutoCommand>();
        services.AddTransient<BaseCommand, SettingsCommand>();

        return services;
    }
    
    /// <summary>
    /// Регистрация фоновых заданий
    /// </summary>
    /// <param name="serviceCollection">serviceCollection</param>
    /// <returns>IServiceCollection</returns>
    public static IServiceCollection AddHostedServices(this IServiceCollection services)
    {
        services.AddHostedService<CheckerHostedService>();
        return services;
    }
    
    public static IServiceCollection AddConfig(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<PaymentsOptions>(configuration.GetSection(new PaymentsOptions().Name));
        services.Configure<RequiredSubscriptionsOptions>(configuration.GetSection(new RequiredSubscriptionsOptions().Name));
        
        return services;
    }
}