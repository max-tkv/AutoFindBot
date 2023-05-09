using AutoFindBot.Abstractions;
using AutoFindBot.Exceptions;
using AutoFindBot.Integration.AutoRu.Exceptions;
using AutoFindBot.Integration.AutoRu.Invariants;
using Microsoft.Extensions.Logging;

namespace AutoFindBot.Integration.AutoRu.HttpMessageHandlers;

public class CheckCaptchaHandler : DelegatingHandler
{
    private readonly ICaptchaSolutionsService _captchaSolutionsService;
    private readonly ILogger<CheckCaptchaHandler> _logger;
    
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
        string requestBody = await request.Content.ReadAsStringAsync(cancellationToken);
        
        var response = await base.SendAsync(request, cancellationToken);
        
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        if (content.IndexOf(AutoRuHttpApiClientInvariants.CaptchaFlag, StringComparison.Ordinal) > 0 || 
            content.IndexOf(AutoRuHttpApiClientInvariants.CaptchaFlagTwo, StringComparison.Ordinal) > 0 ||
            !response.IsSuccessStatusCode)
        {
            await _captchaSolutionsService.SolutionAsync(request);
            request.Content = new StringContent(requestBody);
            
            var checkResponse = await base.SendAsync(request, cancellationToken);
            var checkContent = await checkResponse.Content.ReadAsStringAsync(cancellationToken);
            if (!checkContent.Contains("SUCCESS"))
            {
                throw new AutoRuHttpApiClientException(checkContent);
            }

            foreach (var requestHeader in request.Headers)
            {
                response.Headers.Add(requestHeader.Key, requestHeader.Value);
            }

            throw new SuccessSolutionAutoRuException(response.Headers);
        }

        return response;
    }
}