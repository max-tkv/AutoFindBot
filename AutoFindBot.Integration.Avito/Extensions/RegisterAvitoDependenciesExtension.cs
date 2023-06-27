using System.Net;
using AutoFindBot.Abstractions.HttpClients;
using AutoFindBot.Helpers;
using AutoFindBot.Integration.Avito.HttpMessageHandlers;
using AutoFindBot.Integration.Avito.Invariants;
using AutoFindBot.Integration.Avito.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;
using Polly.Retry;

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
        services
            .AddTransient<CheckSuccessfulStatusCodeMessageHandler>()
            .AddHttpClient<IAvitoHttpApiClient, AvitoHttpApiClient>((serviceProvider, httpClient) =>
            {
                var options = serviceProvider.GetRequiredService<AvitoHttpApiClientOptions>();
                httpClient.BaseAddress = new Uri(options.BaseUrl);
                httpClient.DefaultRequestHeaders.Add("Host", options.Host);
            })
            .AddHttpMessageHandlers(new List<Func<IServiceProvider, DelegatingHandler>>
            {
                container => container.GetRequiredService<CheckSuccessfulStatusCodeMessageHandler>()
            })
            .AddPolicyHandler(CreateRetryPolicy(services));

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
                TimeSpan.FromSeconds(3)
            }, (exception, timeSpan, retryCount, context) =>
            {
                var serviceProvider = services.BuildServiceProvider();
                var logger = serviceProvider.GetService<ILogger<AvitoHttpApiClient>>();
                logger!.LogWarning(
                    $"Retry {retryCount} of {context.PolicyKey} at {timeSpan.TotalSeconds} seconds due to: {exception.Exception.Message}");
            });
    }
}