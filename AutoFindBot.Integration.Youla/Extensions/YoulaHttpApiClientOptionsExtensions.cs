using System.Net;
using Ardalis.GuardClauses;
using AutoFindBot.Abstractions.HttpClients;
using AutoFindBot.Integration.Youla.HttpMessageHandlers;
using AutoFindBot.Integration.Youla.Invariants;
using AutoFindBot.Integration.Youla.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;
using Polly.Retry;

namespace AutoFindBot.Integration.Youla.Extensions;

public static class YoulaHttpApiClientOptionsExtensions
{
    public static IServiceCollection AddYoulaHttpApiClient(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        Guard.Against.Null(services, nameof(services));
        Guard.Against.Null(configuration, nameof(configuration));

        return services
            .RegisterYoulaApiClient()
            .RegisterYoulaHttpApiClientOptions(
                configuration,
                RegisterYoulaHttpApiClientInvariants.OptionsSectionPath);
    }

    private static IServiceCollection RegisterYoulaHttpApiClientOptions(
        this IServiceCollection services,
        IConfiguration configuration,
        string optionsSectionPath)
    {
        var options = configuration
            .GetSection(optionsSectionPath)
            .Get<YoulaHttpApiClientOptions>();
        
        if (options is null)
        {
            throw new InvalidOperationException(
                RegisterYoulaHttpApiClientInvariants.OptionsSectionNotDefined);
        }
        
        options.Validate();

        return services.AddSingleton(options);
    }

    private static IServiceCollection RegisterYoulaApiClient(
        this IServiceCollection services)
    {
        services
            .AddTransient<CheckSuccessfulStatusCodeMessageHandler>()
            .AddHttpClient<IYoulaHttpApiClient, YoulaHttpApiClient>((serviceProvider, httpClient) => 
            {
                var options = serviceProvider.GetRequiredService<YoulaHttpApiClientOptions>();
                httpClient.BaseAddress = new Uri(options.BaseUrl);
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
            .OrResult(response => response.StatusCode == HttpStatusCode.NotFound)
            .WaitAndRetryAsync(new[]
            {
                TimeSpan.FromSeconds(3),
                TimeSpan.FromSeconds(3),
                TimeSpan.FromSeconds(3)
            }, (exception, timeSpan, retryCount, context) =>
            {
                var serviceProvider = services.BuildServiceProvider();
                var logger = serviceProvider.GetService<ILogger<YoulaHttpApiClient>>();
                logger!.LogInformation(
                    $"Retry {retryCount} of {context.PolicyKey} at {timeSpan.TotalSeconds} seconds due to: " +
                    $"{YoulaHttpApiClientInvariants.CaptchaNotReadyMessage}");
            });
    }
}