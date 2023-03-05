using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AutoFindBot.Abstractions.HttpClients;
using AutoFindBot.Integration.Clients;
using AutoFindBot.Integration.Options;
using AutoFindBot.Utils.Http;

namespace AutoFindBot.Integration;

public static class Entry
{
    public static IServiceCollection AddIntegration(this IServiceCollection serviceCollection,
        IConfiguration configuration)
    {
        serviceCollection.AddTradeDealerClient<TradeDealerClientOptions>(options =>
        {
            options.BaseUrl = configuration["TradeDealerClient:BaseUrl"];
            options.Host = "";
        });

        return serviceCollection;
    }
    
    /// <summary>
    /// Регистрация зависимостей для TradeDealerClient
    /// </summary>
    /// <param name="serviceCollection">serviceCollection</param>
    /// <param name="optionsAction">API-client options action</param>
    /// <param name="configuration"></param>
    /// <returns>serviceCollection</returns>
    public static IServiceCollection AddTradeDealerClient<TOptions>(
        [NotNull] this IServiceCollection serviceCollection,
        Action<TOptions> optionsAction) where TOptions : HttpApiClientOptions, new()
    {
        var options = new TOptions();
        optionsAction?.Invoke(options);
    
        serviceCollection
            // register http client
            .AddHttpClient<ITradeDealerClient, TradeDealerClient>(client =>
            {
                ArgumentNullException.ThrowIfNull(options.BaseUrl);
                
                client.BaseAddress = new Uri(options.BaseUrl);
                client.Timeout = options.Timeout;
            });
    
        return serviceCollection;
    }
}