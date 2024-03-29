﻿using AutoFindBot.Abstractions;
using AutoFindBot.Exceptions;
using AutoFindBot.Integration.Drom.Exceptions;
using AutoFindBot.Integration.Drom.Invariants;
using Microsoft.Extensions.Logging;

namespace AutoFindBot.Integration.Drom.HttpMessageHandlers;

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
        if (content.IndexOf(DromHttpApiClientInvariants.CaptchaFlag, StringComparison.Ordinal) > 0 || 
            !response.IsSuccessStatusCode)
        {
            await _captchaSolutionsService.SolutionDromAsync(request, cancellationToken);

            var checkResponse = await base.SendAsync(request, cancellationToken);
            var checkContent = await checkResponse.Content.ReadAsStringAsync(cancellationToken);
            if (!checkContent.Contains(DromHttpApiClientInvariants.SuccessFlag))
            {
                throw new DromHttpApiClientException(checkContent);
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