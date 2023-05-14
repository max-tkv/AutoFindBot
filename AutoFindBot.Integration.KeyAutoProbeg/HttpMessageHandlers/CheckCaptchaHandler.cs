using AutoFindBot.Abstractions;
using AutoFindBot.Exceptions;
using AutoFindBot.Integration.KeyAutoProbeg.Exceptions;
using AutoFindBot.Integration.KeyAutoProbeg.Invariants;
using Microsoft.Extensions.Logging;

namespace AutoFindBot.Integration.KeyAutoProbeg.HttpMessageHandlers;

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
        var response = await base.SendAsync(request, cancellationToken);

        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        if (content.IndexOf(KeyAutoProbegHttpApiClientInvariants.CaptchaFlag, StringComparison.Ordinal) > 0 || 
            !response.IsSuccessStatusCode)
        {
            await _captchaSolutionsService.SolutionKeyAutoProbegAsync(request, cancellationToken);

            var checkResponse = await base.SendAsync(request, cancellationToken);
            var checkContent = await checkResponse.Content.ReadAsStringAsync(cancellationToken);
            if (!checkContent.Contains(KeyAutoProbegHttpApiClientInvariants.SuccessFlag))
            {
                throw new KeyAutoProbegHttpApiClientException(checkContent);
            }

            foreach (var requestHeader in request.Headers)
            {
                response.Headers.Add(requestHeader.Key, requestHeader.Value);
            }

            throw new SuccessSolutionException(response.Headers);
        }

        return response;
    }
}