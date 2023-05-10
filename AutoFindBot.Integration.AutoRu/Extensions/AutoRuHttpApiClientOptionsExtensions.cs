using System.Net;
using AutoFindBot.Abstractions.HttpClients;
using AutoFindBot.Helpers;
using AutoFindBot.Integration.AutoRu.Exceptions;
using AutoFindBot.Integration.AutoRu.HttpMessageHandlers;
using AutoFindBot.Integration.AutoRu.Invariants;
using AutoFindBot.Integration.AutoRu.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;
using Polly.Retry;

namespace AutoFindBot.Integration.AutoRu.Extensions;

public static class AutoRuHttpApiClientOptionsExtensions
{
    public static IServiceCollection AddAutoRuHttpApiClient(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        Guard.NotNull(services, nameof(services));
        Guard.NotNull(configuration, nameof(configuration));

        return services
            .RegisterAutoRuApiClient()
            .RegisterAutoRuHttpApiClientOptions(
                configuration,
                RegisterAutoRuHttpApiClientInvariants.OptionsSectionPath);
    }

    private static IServiceCollection RegisterAutoRuHttpApiClientOptions(
        this IServiceCollection services,
        IConfiguration configuration,
        string optionsSectionPath)
    {
        var options = configuration
            .GetSection(optionsSectionPath)
            .Get<AutoRuHttpApiClientOptions>();
        
        if (options is null)
        {
            throw new InvalidOperationException(
                RegisterAutoRuHttpApiClientInvariants.OptionsSectionNotDefined);
        }
        
        options.Validate();

        return services.AddSingleton(options);
    }

    private static IServiceCollection RegisterAutoRuApiClient(
        this IServiceCollection services)
    {
        var retryPolicy = CreateRetryPolicy(services).Result;
        services
            .AddTransient<CheckSuccessfulStatusCodeMessageHandler>()
            .AddTransient<CheckCaptchaHandler>()
            .AddHttpClient<IAutoRuHttpApiClient, AutoRuHttpApiClient>((serviceProvider, httpClient) =>
            {
                var options = serviceProvider.GetRequiredService<AutoRuHttpApiClientOptions>();
                httpClient.BaseAddress = new Uri(options.BaseUrl);
            })
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                var handler = new HttpClientHandler();
                handler.UseCookies = true;
                handler.AllowAutoRedirect = true;
                
                return handler;
            })
            .AddHttpMessageHandlers(new List<Func<IServiceProvider, DelegatingHandler>>
            {
                container => container.GetRequiredService<CheckSuccessfulStatusCodeMessageHandler>(),
                container => container.GetRequiredService<CheckCaptchaHandler>()
            });
            //.AddPolicyHandler(retryPolicy);

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
                TimeSpan.FromSeconds(3)
            }, async (exception, timeSpan, retryCount, context) =>
            {
                var serviceProvider = services.BuildServiceProvider();
                var logger = serviceProvider.GetService<ILogger<AutoRuHttpApiClient>>();
                logger!.LogWarning(
                    $"Retry {retryCount} of {context.PolicyKey} at {timeSpan.TotalSeconds} seconds due to: " +
                    $"{exception?.Exception?.Message ?? await exception?.Result?.Content.ReadAsStringAsync()}");
            });
    }
}