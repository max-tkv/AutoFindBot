using AutoFindBot.Abstractions.HttpClients;
using AutoFindBot.Helpers;
using AutoFindBot.Integration.Avito.Invariants;
using AutoFindBot.Integration.Avito.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AutoFindBot.Integration.Avito.Extensions;

public static class RegisterAvitoDependenciesExtension
{
    public static IServiceCollection AddAvitoHttpApiClient(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        Guard.NotNull(services, nameof(services));
        Guard.NotNull(configuration, nameof(configuration));

        return services
            .RegisterAvitoApiClient()
            .RegisterAvitoHttpApiClientOptions(
                configuration,
                RegisterAvitoHttpApiClientInvariants.OptionsSectionPath);
    }

    private static IServiceCollection RegisterAvitoHttpApiClientOptions(
        this IServiceCollection services,
        IConfiguration configuration,
        string optionsSectionPath)
    {
        var options = configuration
            .GetSection(optionsSectionPath)
            .Get<AvitoHttpApiClientOptions>();
        
        if (options is null)
        {
            throw new InvalidOperationException(
                RegisterAvitoHttpApiClientInvariants.OptionsSectionNotDefined);
        }
        
        options.Validate();

        return services.AddSingleton(options);
    }

    private static IServiceCollection RegisterAvitoApiClient(
        this IServiceCollection services)
    {
        services.AddHttpClient<IAvitoHttpApiClient, AvitoHttpApiClient>((serviceProvider, httpClient) =>
        {
            var options = serviceProvider.GetService<AvitoHttpApiClientOptions>();
            httpClient.BaseAddress = new Uri(options.BaseUrl);
        });

        return services;
    }
}