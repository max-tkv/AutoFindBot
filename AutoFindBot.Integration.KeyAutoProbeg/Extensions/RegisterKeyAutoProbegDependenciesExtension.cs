using System.Net.Http.Headers;
using AutoFindBot.Abstractions.HttpClients;
using AutoFindBot.Helpers;
using AutoFindBot.Integration.KeyAutoProbeg.HttpMessageHandlers;
using AutoFindBot.Integration.KeyAutoProbeg.Invariants;
using AutoFindBot.Integration.KeyAutoProbeg.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;
using Polly.Retry;

namespace AutoFindBot.Integration.KeyAutoProbeg.Extensions;

public static class RegisterKeyAutoProbegDependenciesExtension
{
    public static IServiceCollection AddKeyAutoProbegHttpApiClient(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        Guard.NotNull(services, nameof(services));
        Guard.NotNull(configuration, nameof(configuration));

        return services
            .RegisterApplicationsApiClient()
            .RegisterKeyAutoProbegHttpApiClientOptions(
                configuration,
                RegisterKeyAutoProbegHttpApiClientInvariants.OptionsSectionPath);
    }

    private static IServiceCollection RegisterKeyAutoProbegHttpApiClientOptions(
        this IServiceCollection services,
        IConfiguration configuration,
        string optionsSectionPath)
    {
        var options = configuration
            .GetSection(optionsSectionPath)
            .Get<KeyAutoProbegHttpApiClientOptions>();
        
        if (options is null)
        {
            throw new InvalidOperationException(
                RegisterKeyAutoProbegHttpApiClientInvariants.OptionsSectionNotDefined);
        }
        
        options.Validate();

        return services.AddSingleton(options);
    }

    private static IServiceCollection RegisterApplicationsApiClient(
        this IServiceCollection services)
    {
        services
            .AddTransient<CheckCaptchaHandler>()
            .AddHttpClient<IKeyAutoProbegHttpApiClient, KeyAutoProbegHttpApiClient>((serviceProvider, httpClient) =>
            {
                var options = serviceProvider.GetRequiredService<KeyAutoProbegHttpApiClientOptions>();
                httpClient.BaseAddress = new Uri(options.BaseUrl);
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/113.0.0.0 Safari/537.36");
            })
            .AddHttpMessageHandlers(new List<Func<IServiceProvider, DelegatingHandler>>
            {
                container => container.GetRequiredService<CheckCaptchaHandler>()
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
                var logger = serviceProvider.GetService<ILogger<KeyAutoProbegHttpApiClient>>();
                logger!.LogWarning(
                    $"Retry {retryCount} of {context.PolicyKey} at {timeSpan.TotalSeconds} seconds due to: {exception.Exception.Message}");
            });
    }
}