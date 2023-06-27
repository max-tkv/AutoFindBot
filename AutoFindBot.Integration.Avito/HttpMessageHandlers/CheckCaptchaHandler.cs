using AutoFindBot.Abstractions;
using AutoFindBot.Exceptions;
using AutoFindBot.Integration.Avito.Invariants;

namespace AutoFindBot.Integration.Avito.HttpMessageHandlers
{
    public class CheckCaptchaHandler : DelegatingHandler
    {
        private ICaptchaSolutionsService _captchaSolutionsService;

        public CheckCaptchaHandler(ICaptchaSolutionsService captchaSolutionsService) 
        {
            _captchaSolutionsService = captchaSolutionsService;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken);

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            if (content.IndexOf(AvitoHttpApiClientInvariants.CaptchaFlag, StringComparison.Ordinal) > 0 ||
                !response.IsSuccessStatusCode)
            {
                await _captchaSolutionsService.SolutionAvitoAsync(request, cancellationToken);

                var checkResponse = await base.SendAsync(request, cancellationToken);
                var checkContent = await checkResponse.Content.ReadAsStringAsync(cancellationToken);
                if (!checkContent.Contains(AvitoHttpApiClientInvariants.SuccessFlag))
                {
                    throw new AvitoHttpApiClientException(checkContent);
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
}
