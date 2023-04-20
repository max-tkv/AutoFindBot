using System.Net.Http.Json;
using System.Net.Sockets;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Web;
using AutoFindBot.Abstractions;
using AutoFindBot.Abstractions.HttpClients;
using AutoFindBot.Exceptions;
using AutoFindBot.Integration.AutoRu.Exceptions;
using AutoFindBot.Integration.AutoRu.Invariants;
using AutoFindBot.Integration.AutoRu.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace AutoFindBot.Integration.AutoRu.HttpMessageHandlers;

public class CheckCaptchaHandler : DelegatingHandler
{
    private readonly ICaptchaSolutionsService _captchaSolutionsService;
    private readonly IRuCaptchaHttpApiClient _ruCaptchaHttpApiClient;
    private readonly ILogger<CheckCaptchaHandler> _logger;

    public CheckCaptchaHandler(ICaptchaSolutionsService captchaSolutionsService, IRuCaptchaHttpApiClient ruCaptchaHttpApiClient, ILogger<CheckCaptchaHandler> logger)
    {
        _captchaSolutionsService = captchaSolutionsService;
        _ruCaptchaHttpApiClient = ruCaptchaHttpApiClient;
        _logger = logger;
    }
    
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        return await base.SendAsync(request, cancellationToken);
        // var content = await response.Content.ReadAsStringAsync();
        
        // if (content.IndexOf(AutoRuHttpApiClientInvariants.CaptchaFlag, StringComparison.Ordinal) > 0)
        // {
        //     _logger.LogWarning($"Зпрос заблокирован. Требуется решить каптчу. " +
        //                        $"Получен ответ: {content}");
        //     
        //     var requestMessage = await SolutionAsync(content);
        //     return await base.SendAsync(requestMessage, cancellationToken);
        // }

        // return await base.SendAsync(request, cancellationToken);
    }
    
    public async Task<HttpRequestMessage> SolutionAsync(string html)
    {
        try
        {
            var pathCaptcha = GetCaptchaPath(html);
            var actionPage = await GetCaptchaActionPageAsync(pathCaptcha);
            var imageCaptchaUrl = GetCaptchaImageUrl(actionPage);
            var imageCaptcha = await DownloadImageAsync(imageCaptchaUrl);
        
            var captchaId = await _ruCaptchaHttpApiClient.SendCaptchaAsync(imageCaptcha);
            var code = await GetResultCaptchaWithRetriesAsync(captchaId);

            return await SendCaptchaCodeAsync(actionPage, code);
        }
        catch (Exception e)
        {
            throw new CaptchaSolutionsServiceException($"Произошла ошибка при решении капчи: {e}");
        }
    }

    private async Task<HttpRequestMessage> SendCaptchaCodeAsync(string actionPage, string code)
    {
        var pathCaptcha = GetCaptchaPath(actionPage);
        //var client = new HttpClient(handler);
        var request = new HttpRequestMessage(HttpMethod.Post, $"https://auto.ru{pathCaptcha}&rep={HttpUtility.UrlEncode(code)}");
        //request.Headers.Add("Cookie", "_csrf_token=2d255fa2112b43736d05e53ea61580a5542f5066b48dd21f; _yasc=s5HWgnRmcABf/i+VVVTZqhZoacJ6Bh49y1uj2jOAWSMSHBvsi81Wb2Kg9dBKaA==; autoru_sid=a%3Ag6438768d2g1ncdbbho5hsceunla0grj.8b3ed8e89fc864c02cd1b08ecacc755c%7C1681421965442.604800.Zwt2-tRRZxys9-LYlIbwTQ.Ft1Mifj2gL_zrtYYtLKO1L8vfzoYHGzKTLW6JKveXZ4; autoruuid=g6438768d2g1ncdbbho5hsceunla0grj.8b3ed8e89fc864c02cd1b08ecacc755c; from=direct; from_lifetime=1681422086017; spravka=dD0xNjgxNDIyMDgxO2k9NS4xNTQuMTgxLjIzO0Q9QjlEQjAyMDNFNzU2NTMzNzFEQTM3OUZDMDJCMTQ1RkEwNDY3M0NCNzFERDYxRTVENzhCMTU1M0U5M0FGQzk4MDgwOTkyQ0I2NUFFRTAxMzc0RjEzMTQwMDgwNzQ7dT0xNjgxNDIyMDgxNjQzNzY5NzQ1O2g9NjBhYTY2OWY0YWFkMmI3OTRlYjk0ZTMzMTRiNWQ5YmE=; suid=2afc6962d68530d543dbe5421e57d6d7.282681e87b11f82df17b6fcad548c8d8");
        var content = new MultipartFormDataContent();
        content.Add(new StringContent(code), "rep");
        content.Add(new StringContent(""), "rdata");
        content.Add(new StringContent(""), "aesKey");
        content.Add(new StringContent(""), "signKey");
        content.Add(new StringContent(""), "pdata");
        content.Add(new StringContent(""), "tdata");
        request.Content = content;
        var response = await base.SendAsync(request, new CancellationToken());

        return response.RequestMessage;
    }

    public async Task<string> GetResultCaptchaWithRetriesAsync(string id, int retryCount = 8, int delaySeconds = 10)
    {
        for (int i = 0; i < retryCount; i++)
        {
            try
            {
                return await _ruCaptchaHttpApiClient.GetResultCaptchaAsync(id);
            }
            catch (Exception ex)
            {
                // Log the error if necessary
                _logger.LogInformation(ex, $"Retry {i + 1} failed: {ex.Message}");

                // Wait for the specified delay before retrying
                await Task.Delay(delaySeconds * 1000);
            }
        }

        throw new CaptchaSolutionsServiceException($"Retry count of {retryCount} exceeded.");
    }

    private async Task<string> GetCaptchaActionPageAsync(string pathCaptcha)
    {
        ArgumentNullException.ThrowIfNull(pathCaptcha);

        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://auto.ru" + pathCaptcha);
            var response = await base.SendAsync(request, new CancellationToken());
            response.EnsureSuccessStatusCode();
                
            return await response.Content.ReadAsStringAsync();
        }
        catch (HttpRequestException ex) when (ex.InnerException is SocketException socketEx)
        {
            throw new CaptchaSolutionsServiceException($"Не удалось подключиться к серверу: {socketEx}");
        }
        catch (HttpRequestException ex)
        {
            throw new CaptchaSolutionsServiceException($"Произошла ошибка получении страницы с капчей: {ex}");
        }
    }
    
    private async Task<byte[]> DownloadImageAsync(string imageUrl)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, imageUrl);
            var response = await base.SendAsync(request, new CancellationToken());
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsByteArrayAsync();
        }
        catch (HttpRequestException ex) when (ex.InnerException is SocketException socketEx)
        {
            throw new CaptchaSolutionsServiceException($"Не удалось подключиться к серверу: {socketEx}");
        }
        catch (HttpRequestException ex)
        {
            throw new CaptchaSolutionsServiceException($"Произошла ошибка при загрузке изображения: {ex}");
        }
    }
    
    public string GetCaptchaImageUrl(string html)
    {
        string pattern = "<div class=\"AdvancedCaptcha-View\"><img src=\"(.*?)\".*?><\\/div>";
        var match = Regex.Match(html, pattern);
        if (match.Success)
        {
            return match.Groups[1].Value;
        }
        
        throw new CaptchaSolutionsServiceException("HTML не содержит капчу");
    }

    private string GetCaptchaPath(string html)
    {
        string pattern = @"action=""([^""]+)""";
        Match match = Regex.Match(html, pattern);
        if (match.Success) 
        {
            return match.Groups[1].Value;
        }
        
        throw new CaptchaSolutionsServiceException("Не удалось определить путь к капче.");
    }
}