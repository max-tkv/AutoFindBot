﻿using AutoFindBot.Abstractions;
using AutoFindBot.Repositories;
using AutoFindBot.Storage.PostgreSql.Invariants;
using AutoFindBot.Storage.PostgreSql.Options;
using AutoFindBot.Storage.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AutoFindBot.Storage.PostgreSql.Extensions;

public static class RegisterPostgreSqlStorageExtension
{
    /// <summary>
    /// Добавления зависимостей для работы с БД
    /// </summary>
    /// <param name="serviceCollection"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection RegisterPostgreSqlStorage(
        this IServiceCollection serviceCollection,
        IConfiguration configuration)
    {
        var options = configuration
            .GetSection(RegisterOptionsPathInvariants.PostgeSQLOptionsPath)
            .Get<PostgreSqlOptions>();

        options.Validate();

        serviceCollection
            .AddEntityFrameworkNpgsql()
            .AddDbContextPool<AppDbContext>(context =>
            {
                context.UseNpgsql(options.ConnectionString);
            });

        return serviceCollection
            .AddTransient<IUnitOfWork, UnitOfWork>()
            .AddTransient<IAppUserRepository, AppUserRepository>()
            .AddTransient<IActionRepository, ActionRepository>()
            .AddTransient<IPaymentRepository, PaymentRepository>()
            .AddTransient<ICarRepository, CarRepository>()
            .AddTransient<IUserFilterRepository, UserFilterRepository>();
    }
}