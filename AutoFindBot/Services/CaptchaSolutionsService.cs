using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Web;
using AutoFindBot.Abstractions;
using AutoFindBot.Abstractions.HttpClients;
using AutoFindBot.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AutoFindBot.Services;

public class CaptchaSolutionsService : ICaptchaSolutionsService
{
    private readonly IRuCaptchaHttpApiClient _ruCaptchaHttpApiClient;
    private readonly string? _baseUrl;
    private readonly ILogger<CaptchaSolutionsService> _logger;

    public CaptchaSolutionsService(
        IRuCaptchaHttpApiClient ruCaptchaHttpApiClient,
        IConfiguration configuration,
        ILogger<CaptchaSolutionsService> logger)
    {
        _ruCaptchaHttpApiClient = ruCaptchaHttpApiClient;
        _logger = logger;
        _baseUrl = configuration.GetValue<string>("Integration:AutoRu:BaseUrl");
        
        ArgumentNullException.ThrowIfNull(_baseUrl);
    }

    public async Task SolutionAsync(string html)
    {
        try
        {
            var pathCaptcha = GetCaptchaPath(html);
            var actionPage = await GetCaptchaActionPageAsync(pathCaptcha);
            var imageCaptchaUrl = GetCaptchaImageUrl(actionPage);
            var imageCaptcha = await DownloadImageAsync(imageCaptchaUrl);
        
            var captchaId = await _ruCaptchaHttpApiClient.SendCaptchaAsync(imageCaptcha);
            var code = await GetResultCaptchaWithRetriesAsync(captchaId);

            await SendCaptchaCodeAsync(actionPage, code);
        }
        catch (Exception e)
        {
            throw new CaptchaSolutionsServiceException($"Произошла ошибка при решении капчи: {e}");
        }
    }

    private async Task SendCaptchaCodeAsync(string actionPage, string code)
    {
        var handler = new HttpClientHandler()
        {
            AllowAutoRedirect = true
        };
        
        var pathCaptcha = GetCaptchaPath(actionPage);
        var client = new HttpClient(handler);
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
        var response = await client.SendAsync(request);
        //response.EnsureSuccessStatusCode();
        var d = await response.Content.ReadAsStringAsync();
        Console.WriteLine();
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
                _logger.LogError(ex, $"Retry {i + 1} failed: {ex.Message}");

                // Wait for the specified delay before retrying
                await Task.Delay(delaySeconds * 1000);
            }
        }

        throw new CaptchaSolutionsServiceException($"Retry count of {retryCount} exceeded.");
    }

    private async Task<string> GetCaptchaActionPageAsync(string pathCaptcha)
    {
        ArgumentNullException.ThrowIfNull(pathCaptcha);

        using (var httpClient = new HttpClient(){ BaseAddress = new Uri(_baseUrl) })
        {
            try
            {
                var response = await httpClient.GetAsync(pathCaptcha, HttpCompletionOption.ResponseContentRead);
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
    }
    
    private async Task<byte[]> DownloadImageAsync(string imageUrl)
    {
        using (var httpClient = new HttpClient())
        {
            try
            {
                var response = await httpClient.GetAsync(imageUrl);
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