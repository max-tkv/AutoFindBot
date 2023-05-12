using Ardalis.GuardClauses;
using AutoFindBot.Abstractions.HttpClients;
using AutoFindBot.Integration.Drom.HttpMessageHandlers;
using AutoFindBot.Integration.Drom.Invariants;
using AutoFindBot.Integration.Drom.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;
using Polly.Retry;

namespace AutoFindBot.Integration.Drom.Extensions;

public static class RegisterDromDependenciesExtension
{
    public static IServiceCollection AddDromHttpApiClient(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        Guard.Against.Null(services, nameof(services));
        Guard.Against.Null(configuration, nameof(configuration));

        return services
            .RegisterApplicationsApiClient()
            .RegisterDromHttpApiClientOptions(
                configuration,
                RegisterDromHttpApiClientInvariants.OptionsSectionPath);
    }

    private static IServiceCollection RegisterDromHttpApiClientOptions(
        this IServiceCollection services,
        IConfiguration configuration,
        string optionsSectionPath)
    {
        var options = configuration
            .GetSection(optionsSectionPath)
            .Get<DromHttpApiClientOptions>();
        
        if (options is null)
        {
            throw new InvalidOperationException(
                RegisterDromHttpApiClientInvariants.OptionsSectionNotDefined);
        }
        
        options.Validate();

        return services.AddSingleton(options);
    }

    private static IServiceCollection RegisterApplicationsApiClient(
        this IServiceCollection services)
    {
        services
            .AddTransient<CheckCaptchaHandler>()
            .AddHttpClient<IDromHttpApiClient, DromHttpApiClient>((serviceProvider, httpClient) =>
            {
                var options = serviceProvider.GetRequiredService<DromHttpApiClientOptions>();
                httpClient.BaseAddress = new Uri(options.BaseUrl);
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
                var logger = serviceProvider.GetService<ILogger<DromHttpApiClient>>();
                logger!.LogWarning(
                    $"Retry {retryCount} of {context.PolicyKey} at {timeSpan.TotalSeconds} seconds due to: {exception.Exception.Message}");
            });
    }
}