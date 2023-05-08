using System.Net;
using AutoFindBot.Abstractions;
using AutoFindBot.Abstractions.HttpClients;
using AutoFindBot.Integration.AutoRu.Exceptions;
using AutoFindBot.Integration.AutoRu.Invariants;
using Microsoft.Extensions.Logging;

namespace AutoFindBot.Integration.AutoRu.HttpMessageHandlers;

public class CheckCaptchaHandler : DelegatingHandler
{
    private readonly ICaptchaSolutionsService _captchaSolutionsService;
    private readonly ILogger<CheckCaptchaHandler> _logger;
    private HttpClient httpClient;
    private CookieContainer cookieContainer;
    
    public CheckCaptchaHandler(
        ICaptchaSolutionsService captchaSolutionsService, 
        ILogger<CheckCaptchaHandler> logger)
    {
        _captchaSolutionsService = captchaSolutionsService;
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        string requestBody = await request.Content.ReadAsStringAsync();
        
        var response = await base.SendAsync(request, cancellationToken);
        
        var content = await response.Content.ReadAsStringAsync();
        if (content.IndexOf(AutoRuHttpApiClientInvariants.CaptchaFlag, StringComparison.Ordinal) > 0 || 
            content.IndexOf(AutoRuHttpApiClientInvariants.CaptchaFlagTwo, StringComparison.Ordinal) > 0)
        {
            await _captchaSolutionsService.SolutionAsync(request);
            request.Content = new StringContent(requestBody);
            response = await base.SendAsync(request, cancellationToken);
            content = await response.Content.ReadAsStringAsync();

            if (!content.Contains("SUCCESS"))
            {
                throw new AutoRuHttpApiClientException(content);
            }
        }

        return response;
    }
}