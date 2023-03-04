using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using AutoFindBot.Abstractions;
using AutoFindBot.Repositories;
using AutoFindBot.Storage.Repositories;

namespace AutoFindBot.Storage;

public static class Entry
{
    private static readonly Action<DbContextOptionsBuilder> DefaultOptionsAction = (options) => { };
        
    /// <summary>
    /// Добавления зависимостей для работы с БД
    /// </summary>
    /// <param name="serviceCollection">serviceCollection</param>
    /// <param name="optionsAction">optionsAction</param>
    /// <returns>IServiceCollection</returns>
    public static IServiceCollection AddSqlStorage(this IServiceCollection serviceCollection, Action<DbContextOptionsBuilder> optionsAction)
    {
        serviceCollection.AddDbContext<AppDbContext>(optionsAction ?? DefaultOptionsAction)
            .AddTransient<IUnitOfWork, UnitOfWork>()
            .AddTransient<IAppUserRepository, AppUserRepository>()
            .AddTransient<IActionRepository, ActionRepository>()
            .AddTransient<IPaymentRepository, PaymentRepository>();
        return serviceCollection;
    }
}