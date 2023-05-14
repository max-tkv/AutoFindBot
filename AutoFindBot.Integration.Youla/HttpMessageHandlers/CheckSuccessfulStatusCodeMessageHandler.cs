using AutoFindBot.Integration.Youla.Exceptions;
using AutoFindBot.Integration.Youla.Invariants;

namespace AutoFindBot.Integration.Youla.HttpMessageHandlers;

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
        
        string errorMessage = YoulaHttpApiClientInvariants.HttpErrorMessage
            .Replace(":requestUri", response.RequestMessage?.RequestUri?.ToString())
            .Replace(":content", await response.Content.ReadAsStringAsync(cancellationToken));
        throw new YoulaHttpApiClientException(errorMessage);
    }
}