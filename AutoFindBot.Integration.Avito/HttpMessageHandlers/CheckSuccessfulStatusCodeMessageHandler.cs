using AutoFindBot.Exceptions;
using AutoFindBot.Integration.Avito.Invariants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoFindBot.Integration.Avito.HttpMessageHandlers
{
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

            string errorMessage = AvitoHttpApiClientInvariants.HttpErrorMessage
                .Replace(":requestUri", response.RequestMessage?.RequestUri?.ToString())
                .Replace(":content", await response.Content.ReadAsStringAsync(cancellationToken));
            throw new AvitoHttpApiClientException(errorMessage);
        }
    }
}
