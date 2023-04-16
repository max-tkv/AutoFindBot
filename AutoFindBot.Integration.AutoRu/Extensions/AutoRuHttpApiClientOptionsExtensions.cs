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
                httpClient.DefaultRequestHeaders.Add("x-csrf-token",
                    "98f791fd232542a3c05f1504583034e23b912758093c5f2c");
                httpClient.DefaultRequestHeaders.Add("cookie",
                    $"suid=868ad5384045751f631c600a8d006b7d.b86ecad58ef4eeefc22d7f5469c3ab58; my=YwA%3D; gdpr=0; yandex_login=max.tkv; hide-proauto-pimple=1; L=UgNFY3R8QV56ckNbfkB2cnJxa1l0UX58PAIzbBIjFw==.1675684764.15245.317052.08f75f7973e4c7cf3a43399cd0c9945d; autoru_sid=70278389%7C1675692102430.7776000.hYJ903pTqWJGNGd0UCb-IQ.NJ4aWyhrNb-BenqoHIHFn2kBnCgehN7XwslodNF_8QQ; autoruuid=g63db9f3c15mhb65c6g6prfvgpdlhkeu.bc3e666d3ef28618d6a6e33d668003a1; popups-autoru-plus-shown-count=1; gids=8; yandexuid=6813776941674329688; popups-popup-pdd-spring-shown-count=1; i=La+V8U//fNNkg00BFKNoVzVBLj3T8IiVALTuyEkkDvAbyEU3AN6pcLGp9s48Ks9itB2UZn1BwgFcY8CY2mTa1NOqATg=; gradius=0; autoru_gdpr=1; crookie=CabMlmQgxTK6l3K83F8YSwewuqcX1cEjWfvHULLMwv08kW8NZVoynX8r5/xbj06703494li0AC6/zO8sPorAHIPk4Bs=; cmtchd=MTY4MTIyMTQxMDM4MQ==; _ym_uid=1675337539281431015; _csrf_token=98f791fd232542a3c05f1504583034e23b912758093c5f2c; yuidlt=1; speaker_lottery_popup=closed; Session_id=3:1681503344.5.0.1674329708913:lBY-Lg:31.1.1:czoxNjY4ODg5OTk5NDY5OmxCWS1MZzo0NA.2:1|1370033367.-1.2|385032602.1068.2.2:1068|61:10012481.368317.aEJOXaSF_5s3y7KrPgWYqsYIL1U; ys=svt.1%23def_bro.1%23ead.2FECB7CF%23wprid.1681421585955859-14324644945501696284-balancer-l7leveler-kubr-yp-vla-44-BAL-7119%23ybzcc.ru%23newsca.native_cache#udn.cDptYXgudGt2#c_chck.55029947; mda2_beacon=1681503344262; _ym_isad=2; from=direct; cycada=vEzLgsy3D7WpdwADSe1WRaw0jK4KMx2ZIMXT+LrYj/0=; spravka=dD0xNjgxNTA3MTk2O2k9NDYuNjIuMjIuMTQ4O0Q9RDkyN0Q1NUM1MEY5RTk1RUExRDJBQUE1Njk1RDI1RTM0NjIyRkJEMEZEODQ1QzA2Mjg4RkVDNDIwMkJBRDg5NTIwOEJEODNGQzFEN0I0ODNFOTQ0MUJFRTIxRjM7dT0xNjgxNTA3MTk2MTc1OTY2OTgyO2g9OGFmMDkwYWU1Y2Q4Mjg3ODBlZWJlOWFkN2FkMDY3M2I=; _ym_d=1681509080; autoru-visits-count=9; count-visits=3; _yasc=iCLO3xCLsW3nfQ6NuoQHhyZmTghO5KHJ3dvqMIiU15Yhp0ct74qAYiSBSIztqUI=; from_lifetime=1681552571951");
            })
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                var handler = new HttpClientHandler
                {
                    CookieContainer = new CookieContainer(),
                    UseCookies = true
                };
                return handler;
            })
            .AddHttpMessageHandlers(new List<Func<IServiceProvider, DelegatingHandler>>
            {
                container => container.GetRequiredService<CheckSuccessfulStatusCodeMessageHandler>(),
                container => container.GetRequiredService<CheckCaptchaHandler>()
            })
            .AddPolicyHandler(retryPolicy);

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
                    $"{nameof(AutoRuHttpApiClient)}: " +
                    $"Retry {retryCount} of {context.PolicyKey} at {timeSpan.TotalSeconds} seconds due to: " +
                    $"{exception?.Exception?.Message ?? await exception?.Result?.Content.ReadAsStringAsync()}");
            });
    }
}