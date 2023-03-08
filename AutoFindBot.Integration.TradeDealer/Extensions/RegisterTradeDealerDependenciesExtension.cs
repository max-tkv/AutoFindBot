using AutoFindBot.Abstractions.HttpClients;
using AutoFindBot.Helpers;
using AutoFindBot.Integration.Invariants;
using AutoFindBot.Integration.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AutoFindBot.Integration.Extensions;

public static class RegisterTradeDealerDependenciesExtension
{
    public static IServiceCollection AddTradeDealerHttpApiClient(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        Guard.NotNull(services, nameof(services));
        Guard.NotNull(configuration, nameof(configuration));

        return services
            .RegisterApplicationsApiClient()
            .RegisterTradeDealerHttpApiClientOptions(
                configuration,
                RegisterTradeDealerHttpApiClientInvariants.OptionsSectionPath);
    }

    private static IServiceCollection RegisterTradeDealerHttpApiClientOptions(
        this IServiceCollection services,
        IConfiguration configuration,
        string optionsSectionPath)
    {
        var options = configuration
            .GetSection(optionsSectionPath)
            .Get<TradeDealerHttpApiClientOptions>();

        if (options is null)
        {
            throw new InvalidOperationException(
                RegisterTradeDealerHttpApiClientInvariants.OptionsSectionNotDefined);
        }

        return services.AddSingleton(options);
    }

    private static IServiceCollection RegisterApplicationsApiClient(
        this IServiceCollection services)
    {
        services.AddHttpClient<ITradeDealerHttpApiClient, TradeDealerHttpApiClient>((serviceProvider, httpClient) =>
        {
            var options = serviceProvider.GetService<TradeDealerHttpApiClientOptions>();
            httpClient.BaseAddress = new Uri(options.BaseUrl);
        });

        return services;
    }
}