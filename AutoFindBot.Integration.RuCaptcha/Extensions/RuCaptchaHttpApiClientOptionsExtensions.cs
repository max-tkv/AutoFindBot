using System.Net;
using AutoFindBot.Abstractions.HttpClients;
using AutoFindBot.Helpers;
using AutoFindBot.Integration.RuCaptcha.HttpMessageHandlers;
using AutoFindBot.Integration.RuCaptcha.Invariants;
using AutoFindBot.Integration.RuCaptcha.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;
using Polly.Retry;

namespace AutoFindBot.Integration.RuCaptcha.Extensions;

public static class RuCaptchaHttpApiClientOptionsExtensions
{
    public static IServiceCollection AddRuCaptchaHttpApiClient(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        Guard.NotNull(services, nameof(services));
        Guard.NotNull(configuration, nameof(configuration));

        return services
            .RegisterRuCaptchaApiClient()
            .RegisterRuCaptchaHttpApiClientOptions(
                configuration,
                RegisterRuCaptchaHttpApiClientInvariants.OptionsSectionPath);
    }

    private static IServiceCollection RegisterRuCaptchaHttpApiClientOptions(
        this IServiceCollection services,
        IConfiguration configuration,
        string optionsSectionPath)
    {
        var options = configuration
            .GetSection(optionsSectionPath)
            .Get<RuCaptchaHttpApiClientOptions>();
        
        if (options is null)
        {
            throw new InvalidOperationException(
                RegisterRuCaptchaHttpApiClientInvariants.OptionsSectionNotDefined);
        }
        
        options.Validate();

        return services.AddSingleton(options);
    }

    private static IServiceCollection RegisterRuCaptchaApiClient(
        this IServiceCollection services)
    {
        var retryPolicy = CreateRetryPolicy(services).Result;
        services
            .AddTransient<CheckSuccessfulStatusCodeMessageHandler>()
            .AddHttpClient<IRuCaptchaHttpApiClient, RuCaptchaHttpApiClient>((serviceProvider, httpClient) => 
            {
                var options = serviceProvider.GetRequiredService<RuCaptchaHttpApiClientOptions>();
                httpClient.BaseAddress = new Uri(options.BaseUrl);
            })
            .AddHttpMessageHandlers(new List<Func<IServiceProvider, DelegatingHandler>>
            {
                container => container.GetRequiredService<CheckSuccessfulStatusCodeMessageHandler>()
            });
            // .AddPolicyHandler(retryPolicy)

        return services;
    }
    
    private async static Task<AsyncRetryPolicy<HttpResponseMessage>> CreateRetryPolicy(IServiceCollection services)
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(response => response.StatusCode == HttpStatusCode.Forbidden)
            .WaitAndRetryAsync(new[]
            {
                TimeSpan.FromSeconds(3),
                TimeSpan.FromSeconds(3),
                TimeSpan.FromSeconds(3),
                TimeSpan.FromSeconds(5),
                TimeSpan.FromSeconds(8)
            }, async (exception, timeSpan, retryCount, context) =>
            {
                var serviceProvider = services.BuildServiceProvider();
                var logger = serviceProvider.GetService<ILogger<RuCaptchaHttpApiClient>>();
                logger!.LogWarning(
                    $"{nameof(RuCaptchaHttpApiClient)}: " +
                    $"Retry {retryCount} of {context.PolicyKey} at {timeSpan.TotalSeconds} seconds due to: " +
                    $"{exception?.Exception?.Message ?? await exception?.Result?.Content.ReadAsStringAsync()}");
            });
    }
}