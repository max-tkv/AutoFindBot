using AutoFindBot.Abstractions.HttpClients;
using AutoFindBot.Helpers;
using AutoFindBot.Integration.Invariants;
using AutoFindBot.Integration.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;
using Polly.Retry;

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
            .RegisterTradeDealerApiClient()
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
        
        options.Validate();
        
        return services.AddSingleton(options);
    }

    private static IServiceCollection RegisterTradeDealerApiClient(
        this IServiceCollection services)
    {
        var retryPolicy = CreateRetryPolicy(services);
        services
            .AddHttpClient<ITradeDealerHttpApiClient, TradeDealerHttpApiClient>((serviceProvider, httpClient) =>
            {
                var options = serviceProvider.GetService<TradeDealerHttpApiClientOptions>();
                httpClient.BaseAddress = new Uri(options.BaseUrl);
            })
            .AddPolicyHandler(retryPolicy);

        return services;
    }
    
    private static AsyncRetryPolicy<HttpResponseMessage> CreateRetryPolicy(IServiceCollection services)
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(new[]
            {
                TimeSpan.FromSeconds(3),
                TimeSpan.FromSeconds(3),
                TimeSpan.FromSeconds(3),
                TimeSpan.FromSeconds(5),
                TimeSpan.FromSeconds(8)
            }, (exception, timeSpan, retryCount, context) =>
            {
                var serviceProvider = services.BuildServiceProvider();
                var logger = serviceProvider.GetService<ILogger<TradeDealerHttpApiClient>>();
                logger!.LogWarning(
                    $"{nameof(TradeDealerHttpApiClient)}: Retry {retryCount} of {context.PolicyKey} at {timeSpan.TotalSeconds} seconds due to: {exception}");
            });
    }
}