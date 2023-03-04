using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using AutoFindBot.Utils.ApiResponse;

namespace AutoFindBot.Utils.Http;

    /// <summary>
    /// Базовый класс API клиента по http
    /// </summary>
    public abstract class HttpApiClient
    {
        protected readonly HttpClient HttpClient;
        protected abstract HttpContent GetContent(object obj);
        protected abstract HttpContent GetContent(string data);
        protected abstract HttpContent GetContent(List<KeyValuePair<string, string>> nvc);
        protected abstract T GetObjectFromResponse<T>(string content);

        protected HttpApiClient(HttpClient httpClient)
        {
            HttpClient = httpClient;
        }

        private bool ValidateRemoteCertificate(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors error)
        {
            if (error == SslPolicyErrors.None)
            {
                return true;
            }

            Console.WriteLine($"X509Certificate [{cert.Subject}] Policy Error: '{error.ToString()}'");

            return false;
        }

        protected virtual ApiResponseResult<TResult> GetFailedResult<TResult>(HttpResponseMessage message)
            where TResult : class
        {
            return ApiResponse.ApiResponse.CreateFailure<TResult>();
        }

        protected virtual TResult? GetFailedResultRaw<TResult>(HttpResponseMessage message)
        {
            return default(TResult);
        }

        protected virtual async Task<HttpResponseMessage> SendAsync(string path, HttpMethod method, HttpContent? content = null)
        {
            var request = new HttpRequestMessage(method, path)
            {
                Content = content,
            };

            return await HttpClient.SendAsync(request);
        }

        protected virtual async Task<ApiResponseResult<TResult>> Get<TResult>(string path)
            where TResult : class
        {
            var response = await SendAsync(path, HttpMethod.Get);
            if (response.IsSuccessStatusCode == false)
            {
                return GetFailedResult<TResult>(response);
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var result = GetObjectFromResponse<ApiResponseResult<TResult>>(responseContent);
            return result;
        }

        protected virtual async Task<TResult> GetRaw<TResult>(string path)
        {
            var response = await SendAsync(path, HttpMethod.Get);
            if (response.IsSuccessStatusCode == false)
            {
                return GetFailedResultRaw<TResult>(response);
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var result = GetObjectFromResponse<TResult>(responseContent);
            return result;
        }

        protected virtual async Task<ApiResponse.ApiResponse> Post(string path, object @object)
        {
            var response = await SendAsync(path, HttpMethod.Post, GetContent(@object));
            if (response.IsSuccessStatusCode == false)
            {
                return GetFailedResult<object>(response);
            }

            var content = await response.Content.ReadAsStringAsync();
            var result = GetObjectFromResponse<ApiResponse.ApiResponse>(content);
            return result;
        }

        protected virtual async Task<ApiResponseResult<TResult>> Post<TResult>(string path, object @object)
            where TResult : class
        {
            var response = await SendAsync(path, HttpMethod.Post, GetContent(@object));
            if (response.IsSuccessStatusCode == false)
            {
                return GetFailedResult<TResult>(response);
            }

            var content = await response.Content.ReadAsStringAsync();
            var result = GetObjectFromResponse<ApiResponseResult<TResult>>(content);
            return result;
        }

        protected virtual async Task<ApiResponseResult<TResult>> Post<TResult>(string path)
            where TResult : class
        {
            var response = await SendAsync(path, HttpMethod.Post);
            if (response.IsSuccessStatusCode == false)
            {
                return GetFailedResult<TResult>(response);
            }

            var content = await response.Content.ReadAsStringAsync();
            var result = GetObjectFromResponse<ApiResponseResult<TResult>>(content);
            return result;
        }

        protected virtual async Task<TResult> PostRaw<TResult>(string path, object @object)
        {
            var response = await SendAsync(path, HttpMethod.Post, GetContent(@object));
            if (response.IsSuccessStatusCode == false)
            {
                return GetFailedResultRaw<TResult>(response);
            }

            var content = await response.Content.ReadAsStringAsync();
            var result = GetObjectFromResponse<TResult>(content);
            return result;
        }
    }