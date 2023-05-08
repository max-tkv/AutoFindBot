using System.Globalization;
using System.Text.RegularExpressions;
using AutoFindBot.Abstractions.HttpClients;
using AutoFindBot.Integration.RuCaptcha.Exceptions;
using AutoFindBot.Integration.RuCaptcha.Invariants;
using AutoFindBot.Integration.RuCaptcha.Options;
using AutoFindBot.Models.Avito;
using AutoFindBot.Utils.Http;
using Microsoft.Extensions.Logging;

namespace AutoFindBot.Integration.RuCaptcha;

public class RuCaptchaHttpApiClient : JsonHttpApiClient, IRuCaptchaHttpApiClient
{
    private readonly RuCaptchaHttpApiClientOptions _options;
    private readonly ILogger<RuCaptchaHttpApiClient> _logger;

    public RuCaptchaHttpApiClient(
        HttpClient httpClient, 
        RuCaptchaHttpApiClientOptions options,
        ILogger<RuCaptchaHttpApiClient> logger) : base(httpClient)
    {
        _options = options;
        _logger = logger;
    }
    
    public async Task<string> SendCaptchaAsync(byte[] file)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(file);
            
            string content;
            using (var request = new HttpRequestMessage(HttpMethod.Post, _options.BaseUrl + _options.InQuery))
            {
                using (var formDataContent = new MultipartFormDataContent())
                {
                    formDataContent.Add(new StringContent(_options.ApiKey), "key");
                    formDataContent.Add(new StreamContent(new MemoryStream(file)), "file", "file.png");

                    request.Content = formDataContent;
                    using (var response = await SendAsync(request))
                    {
                        if (response.IsSuccessStatusCode == false)
                        {
                            throw new Exception($"Произошла ошибка: {response}");
                        }
                    
                        content = await response.Content.ReadAsStringAsync();
                    }
                }   
            }

            var id = content.Split("|").Last();

            ArgumentNullException.ThrowIfNull(id);
            
            return id;
        }
        catch (Exception e)
        {
            _logger.LogError($"{nameof(RuCaptchaHttpApiClient)}: {e.Message}");
            throw;
        }
    }

    public async Task<string> GetResultCaptchaAsync(string id)
    {
        ArgumentNullException.ThrowIfNull(id);

        var path = _options.BaseUrl + _options.ResQuery
            .Replace(RuCaptchaHttpApiClientInvariants.ApiKey, _options.ApiKey)
            .Replace(RuCaptchaHttpApiClientInvariants.Id, id);

        var response = await SendAsync(path, HttpMethod.Get);
        var content = await response.Content.ReadAsStringAsync();
        if (response.IsSuccessStatusCode == false)
        {
            throw new Exception($"Произошла ошибка: {content}");
        }

        var result = content.Split("|").Last();

        ArgumentNullException.ThrowIfNull(result);

        return result;
    }
}