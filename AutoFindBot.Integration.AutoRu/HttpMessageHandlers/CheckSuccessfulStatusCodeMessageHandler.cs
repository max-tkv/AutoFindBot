using AutoFindBot.Integration.AutoRu.Exceptions;

namespace AutoFindBot.Integration.AutoRu.HttpMessageHandlers;

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

        string errorMessage = $"Произошла ошибка при вызове метода {response.RequestMessage?.RequestUri}. " +
                              $"Получен ответ: {await response.Content.ReadAsStringAsync(cancellationToken)}";

        throw new AutoRuHttpApiClientException(errorMessage);
    }
}