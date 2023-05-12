using AutoFindBot.Exceptions;
using AutoFindBot.Integration.RuCaptcha.Exceptions;
using AutoFindBot.Integration.RuCaptcha.Invariants;

namespace AutoFindBot.Integration.RuCaptcha.HttpMessageHandlers;

public class CheckSuccessfulStatusCodeMessageHandler : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var response = await base.SendAsync(request, cancellationToken);
        if (response.IsSuccessStatusCode)
        {
            return response;
        }
        
        string errorMessage = RuCaptchaHttpApiClientInvariants.HttpErrorMessage
            .Replace(":requestUri", response.RequestMessage?.RequestUri?.ToString())
            .Replace(":content", await response.Content.ReadAsStringAsync(cancellationToken));
        throw new RuCaptchaHttpApiClientException(errorMessage);
    }
}