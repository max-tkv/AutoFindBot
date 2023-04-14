using AutoFindBot.Integration.RuCaptcha.Exceptions;

namespace AutoFindBot.Integration.RuCaptcha.HttpMessageHandlers;

public class CheckSuccessfulStatusCodeMessageHandler : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var response = await base.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            string errorMessage = $"Произошла ошибка при вызове метода {response.RequestMessage?.RequestUri}. " +
                                  $"Получен ответ: {await response.Content.ReadAsStringAsync(cancellationToken)}";
            throw new RuCaptchaHttpApiClientException(errorMessage);
        }
        
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        if (!content.Contains("OK|"))
        {
            string errorMessage = $"Произошла ошибка при вызове метода {response.RequestMessage?.RequestUri}. " +
                                  $"Получен ответ: {content}";
            throw new RuCaptchaHttpApiClientException(errorMessage);
        }

        return response;
    }
}