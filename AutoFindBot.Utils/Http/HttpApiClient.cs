namespace AutoFindBot.Utils.Http;

    /// <summary>
    /// Базовый класс API клиента по http
    /// </summary>
    public abstract class HttpApiClient
    {
        protected readonly HttpClient HttpClient;

        protected HttpApiClient(HttpClient httpClient)
        {
            HttpClient = httpClient;
        }
        

        protected virtual async Task<HttpResponseMessage> SendAsync(string path, HttpMethod method, HttpContent? content = null)
        {
            var request = new HttpRequestMessage(method, path)
            {
                Content = content,
            };

            return await HttpClient.SendAsync(request);
        }
    }