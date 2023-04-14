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
                httpClient.DefaultRequestHeaders.Add("authority", "auto.ru");
                httpClient.DefaultRequestHeaders.Add("accept-language", "ru,en;q=0.9");
                httpClient.DefaultRequestHeaders.Add("cache-control", "max-age=0");
                httpClient.DefaultRequestHeaders.Add("referer", "https://auto.ru/");
                httpClient.DefaultRequestHeaders.Add("cookie", "_csrf_token=2d255fa2112b43736d05e53ea61580a5542f5066b48dd21f; _yasc=u6lR5xOioYq0dGAvCvBoK4rAHiRb83X2eVVyd3LEX8s9T9CyzpQj2ggqCPDulL0=; _ym_d=1681427440; _ym_uid=1675337539281431015; autoru_sid=a%3Ag6438768d2g1ncdbbho5hsceunla0grj.8b3ed8e89fc864c02cd1b08ecacc755c%7C1681421965442.604800.Zwt2-tRRZxys9-LYlIbwTQ.Ft1Mifj2gL_zrtYYtLKO1L8vfzoYHGzKTLW6JKveXZ4; autoruuid=g6438768d2g1ncdbbho5hsceunla0grj.8b3ed8e89fc864c02cd1b08ecacc755c; from=direct; from_lifetime=1681427440069; spravka=dD0xNjgxNDIyMDgxO2k9NS4xNTQuMTgxLjIzO0Q9QjlEQjAyMDNFNzU2NTMzNzFEQTM3OUZDMDJCMTQ1RkEwNDY3M0NCNzFERDYxRTVENzhCMTU1M0U5M0FGQzk4MDgwOTkyQ0I2NUFFRTAxMzc0RjEzMTQwMDgwNzQ7dT0xNjgxNDIyMDgxNjQzNzY5NzQ1O2g9NjBhYTY2OWY0YWFkMmI3OTRlYjk0ZTMzMTRiNWQ5YmE=; suid=2afc6962d68530d543dbe5421e57d6d7.282681e87b11f82df17b6fcad548c8d8");
            })
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                var handler = new HttpClientHandler
                {
                    CookieContainer = new CookieContainer(),
                    UseCookies = true,
                    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
                };
                handler.CookieContainer.SetCookies(new Uri("https://auto.ru/"),
                    "_csrf_token=2d255fa2112b43736d05e53ea61580a5542f5066b48dd21f; _yasc=u6lR5xOioYq0dGAvCvBoK4rAHiRb83X2eVVyd3LEX8s9T9CyzpQj2ggqCPDulL0=; _ym_d=1681427440; _ym_uid=1675337539281431015; autoru_sid=a%3Ag6438768d2g1ncdbbho5hsceunla0grj.8b3ed8e89fc864c02cd1b08ecacc755c%7C1681421965442.604800.Zwt2-tRRZxys9-LYlIbwTQ.Ft1Mifj2gL_zrtYYtLKO1L8vfzoYHGzKTLW6JKveXZ4; autoruuid=g6438768d2g1ncdbbho5hsceunla0grj.8b3ed8e89fc864c02cd1b08ecacc755c; from=direct; from_lifetime=1681427440069; spravka=dD0xNjgxNDIyMDgxO2k9NS4xNTQuMTgxLjIzO0Q9QjlEQjAyMDNFNzU2NTMzNzFEQTM3OUZDMDJCMTQ1RkEwNDY3M0NCNzFERDYxRTVENzhCMTU1M0U5M0FGQzk4MDgwOTkyQ0I2NUFFRTAxMzc0RjEzMTQwMDgwNzQ7dT0xNjgxNDIyMDgxNjQzNzY5NzQ1O2g9NjBhYTY2OWY0YWFkMmI3OTRlYjk0ZTMzMTRiNWQ5YmE=; suid=2afc6962d68530d543dbe5421e57d6d7.282681e87b11f82df17b6fcad548c8d8");
                return handler;
            })
            .AddHttpMessageHandlers(new List<Func<IServiceProvider, DelegatingHandler>>
            {
                container => container.GetRequiredService<CheckSuccessfulStatusCodeMessageHandler>(),
                container => container.GetRequiredService<CheckCaptchaHandler>()
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
                var logger = serviceProvider.GetService<ILogger<AutoRuHttpApiClient>>();
                logger!.LogWarning(
                    $"{nameof(AutoRuHttpApiClient)}: " +
                    $"Retry {retryCount} of {context.PolicyKey} at {timeSpan.TotalSeconds} seconds due to: " +
                    $"{exception?.Exception?.Message ?? await exception?.Result?.Content.ReadAsStringAsync()}");
            });
    }
}