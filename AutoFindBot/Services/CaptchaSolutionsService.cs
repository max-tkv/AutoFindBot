using System.Collections.ObjectModel;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using Ardalis.GuardClauses;
using AutoFindBot.Abstractions;
using AutoFindBot.Abstractions.HttpClients;
using AutoFindBot.Exceptions;
using AutoFindBot.Invariants;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;

namespace AutoFindBot.Services;

public class CaptchaSolutionsService : ICaptchaSolutionsService
{
    private readonly IRuCaptchaHttpApiClient _ruCaptchaHttpApiClient;
    private readonly ILogger<CaptchaSolutionsService> _logger;
    private readonly IWebDriverService _webDriverService;
    private readonly IConfiguration _configuration;

    public CaptchaSolutionsService(
        IRuCaptchaHttpApiClient ruCaptchaHttpApiClient,
        IConfiguration configuration,
        ILogger<CaptchaSolutionsService> logger,
        IWebDriverService webDriverService)
    {
        _configuration = configuration;
        _ruCaptchaHttpApiClient = ruCaptchaHttpApiClient;
        _logger = logger;
        _webDriverService = webDriverService;
    }

    public async Task SolutionAutoRuAsync(HttpRequestMessage httpRequestMessage)
    {
        try
        {
            var baseUrl = Guard.Against.Null(_configuration.GetValue<string>("Integration:AutoRu:BaseUrl"));
            var getAutoByFilterQuery = Guard.Against.Null(_configuration.GetValue<string>("Integration:AutoRu:GetAutoByFilterQuery"));

            using var driver = _webDriverService.CreateChromeDriver();
            driver.Navigate().GoToUrl(httpRequestMessage.RequestUri?.ToString());

            await _webDriverService.ClickElementByCssSelectorAsync("button[data-id='button-all']", false);
            await _webDriverService.ClickElementByIdAsync("js-button");

            try
            {
                await _webDriverService.ClickElementByIdAsync("confirm-button");
            }
            catch (NotFoundWebElementException)
            {
                if (driver.Url.Contains(getAutoByFilterQuery))
                {
                    SetAutoRuCookiesAndToken(httpRequestMessage, driver.Manage().Cookies.AllCookies);
                    SetRedirect(httpRequestMessage, HttpMethod.Post, driver.Url);
                    
                    return;
                }
            }

            var actionPage = driver.PageSource;
            var imageCaptchaUrl = GetCaptchaImageUrl(actionPage);
            var imageCaptcha = await DownloadImageAsync(imageCaptchaUrl);
        
            var captchaId = await _ruCaptchaHttpApiClient.SendCaptchaAsync(imageCaptcha);
            var code = await _ruCaptchaHttpApiClient.GetResultCaptchaAsync(captchaId);

            await _webDriverService.SendKeysByIdAsync("xuniq-0-1", code);
            await _webDriverService.ClickElementByCssSelectorAsync("button[data-testid='submit']");
            await _webDriverService.ClickElementByIdAsync("confirm-button");

            if (!driver.Url.Contains(getAutoByFilterQuery))
            {
                throw new CaptchaSolutionsServiceException(CaptchaInvariants.ErrorCaptchaSolveMessage);
            }

            SetAutoRuCookiesAndToken(httpRequestMessage, driver.Manage().Cookies.AllCookies);
            SetRedirect(httpRequestMessage, HttpMethod.Post, driver.Url);
        }
        catch (Exception e)
        {
            throw new CaptchaSolutionsServiceException(CaptchaInvariants.SolvingError
                .Replace(":errorMessage", e.Message));
        }
    }
    
    public async Task SolutionKeyAutoProbegAsync(HttpRequestMessage httpRequestMessage)
    {
        using var driver = _webDriverService.CreateChromeDriver();
        driver.Navigate().GoToUrl(httpRequestMessage.RequestUri?.ToString());

        await _webDriverService.FindElementByIdAsync("app");

        SetKeyAutoProbegCookies(httpRequestMessage, driver.Manage().Cookies.AllCookies);
    }

    public Task SolutionDromAsync(HttpRequestMessage request)
    {
        throw new NotImplementedException();
    }

    private void SetRedirect(HttpRequestMessage httpRequest, HttpMethod newMethod, string newUrl)
    {
        Guard.Against.Null(httpRequest, nameof(httpRequest));
        Guard.Against.Null(newMethod, nameof(newMethod));
        Guard.Against.Null(newUrl, nameof(newUrl));
        
        httpRequest.Method = newMethod;
        httpRequest.RequestUri = new Uri(newUrl);
    }
    
    private void SetKeyAutoProbegCookies(HttpRequestMessage httpRequestMessage, ReadOnlyCollection<Cookie> cookiesAllCookies)
    {
        var cookieValue = string.Join("; ", cookiesAllCookies.OrderBy(x => x.Name).Select(cookie => $"{cookie.Name}={cookie.Value}"));
        httpRequestMessage.Headers.Add("Cookie", cookieValue);
    }

    private void SetAutoRuCookiesAndToken(HttpRequestMessage httpRequest, ReadOnlyCollection<Cookie> driverCookies)
    {
        var cookieValue = string.Join("; ", driverCookies.Select(cookie => $"{cookie.Name}={cookie.Value}"));
        cookieValue += "; yandex_login=max.tkv; Session_id=3:1683636553.5.0.1674329708913:lBY-Lg:31.1.1:czoxNjY4ODg5OTk5NDY5OmxCWS1MZzo0NA.2:1|1370033367.-1.2|385032602.1068.2.2:1068|61:10013073.835889.KOkFiqyyTGSqqXv5GqmuUm-8qMg; " +
                       "ys=udn.cDptYXgudGt2#c_chck.3875977813; mda2_beacon=1683636553303; " +
                       "sso_status=sso.passport.yandex.ru:synchronized; " +
                       "_yasc=X23JRT14qotbUdi5k+Ba23UzhUJN6lkNs8ZZdaJROgdeAS95rlxo8Tw/Z4+K2TE=; gradius=0; " +
                       "cycada=8jSDnd6bpcmIU+hlcyNrire9a+uIKehYp+cFw1YoWmI=; _ym_d=1683636760; count-visits=4; " +
                       "from_lifetime=1683637032901";
        httpRequest.Headers.Add("Cookie", cookieValue);
        httpRequest.Headers.Add("x-csrf-token", driverCookies.Single(x => x.Name == "_csrf_token").Value);
    }

    private async Task<byte[]> DownloadImageAsync(string imageUrl)
    {
        try
        {
            using (var httpClient = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Get, imageUrl);
                var response = await httpClient.SendAsync(request, new CancellationToken());
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsByteArrayAsync();   
            }
        }
        catch (HttpRequestException ex) when (ex.InnerException is SocketException socketEx)
        {
            throw new CaptchaSolutionsServiceException(CaptchaInvariants.MessageServerConnection
                .Replace(":errorMessage", socketEx.Message));
        }
        catch (HttpRequestException ex)
        {
            throw new CaptchaSolutionsServiceException(CaptchaInvariants.ImageLoadingError
                .Replace(":errorMessage", ex.Message));
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
        
        throw new CaptchaSolutionsServiceException(CaptchaInvariants.DetectionErrorInHtml);
    }
}