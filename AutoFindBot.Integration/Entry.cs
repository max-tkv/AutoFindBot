using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AutoFindBot.Abstractions;
using AutoFindBot.Integration.Clients;
using AutoFindBot.Utils.Http;

namespace AutoFindBot.Integration;

public static class Entry
{
    public static IServiceCollection AddIntegration(this IServiceCollection serviceCollection,
        IConfiguration configuration)
    {
        // serviceCollection.AddAlgoliaClient<HttpApiClientOptions>(options =>
        // {
        //     options.BaseUrl = configuration["Algolia:BaseUrl"];
        // }, configuration);

        return serviceCollection;
    }
    
    /// <summary>
    /// Регистрация зависимостей для AlgoliaClient
    /// </summary>
    /// <param name="serviceCollection">serviceCollection</param>
    /// <param name="optionsAction">API-client options action</param>
    /// <param name="configuration"></param>
    /// <returns>serviceCollection</returns>
    // public static IServiceCollection AddAlgoliaClient<TOptions>(
    //     [NotNull] this IServiceCollection serviceCollection,
    //     Action<TOptions> optionsAction, IConfiguration configuration)
    //     where TOptions : HttpApiClientOptions, new()
    // {
    //     var options = new TOptions();
    //     optionsAction?.Invoke(options);
    //
    //     serviceCollection
    //         // register http client
    //         .AddHttpClient<IAlgoliaClient, AlgoliaClient>(client =>
    //         {
    //             ArgumentNullException.ThrowIfNull(options.BaseUrl);
    //             
    //             client.BaseAddress = new Uri(options.BaseUrl);
    //             client.Timeout = options.Timeout;
    //         });
    //
    //     return serviceCollection;
    // }
}