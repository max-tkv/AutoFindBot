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

    public async Task SolutionAutoRuAsync(
        HttpRequestMessage httpRequestMessage, 
        CancellationToken stoppingToken = default)
    {
        try
        {
            var filterQuery = Guard.Against.Null(_configuration.GetValue<string>("Integration:AutoRu:GetAutoByFilterQuery"));

            using var driver = _webDriverService.CreateChromeDriver();
            driver.ResetInputState();
            driver.Navigate().GoToUrl(httpRequestMessage.RequestUri);

            await _webDriverService.ClickElementByCssSelectorAsync(
                "button[data-id='button-all']", false, stoppingToken);
            await _webDriverService.ClickElementByIdAsync(
                "js-button", false, stoppingToken);

            string imageCaptchaUrl = "";
            try
            {
                await _webDriverService.ClickElementByIdAsync(
                    "confirm-button", stoppingToken: stoppingToken);
                imageCaptchaUrl = GetCaptchaImageUrl(driver.PageSource);
            }
            catch (NotFoundWebElementException)
            {
                if (driver.Url.Contains(filterQuery))
                {
                    SetAutoRuCookiesAndToken(httpRequestMessage, driver.Manage().Cookies.AllCookies);
                    SetRedirect(httpRequestMessage, HttpMethod.Post, driver.Url);

                    return;
                }
            }
            catch (CaptchaSolutionsServiceException)
            {
                if (driver.Url.Contains(filterQuery))
                {
                    SetAutoRuCookiesAndToken(httpRequestMessage, driver.Manage().Cookies.AllCookies);
                    SetRedirect(httpRequestMessage, HttpMethod.Post, driver.Url);

                    return;
                }

                throw;
            }
            
            var imageCaptcha = await DownloadImageAsync(imageCaptchaUrl, stoppingToken);
        
            var captchaId = await _ruCaptchaHttpApiClient.SendCaptchaAsync(imageCaptcha);
            var code = await _ruCaptchaHttpApiClient.GetResultCaptchaAsync(captchaId);

            await _webDriverService.SendKeysByIdAsync(
                "xuniq-0-1", code, stoppingToken: stoppingToken);
            await _webDriverService.ClickElementByCssSelectorAsync(
                "button[data-testid='submit']", stoppingToken: stoppingToken);
            await _webDriverService.ClickElementByIdAsync(
                "confirm-button", stoppingToken: stoppingToken);

            if (!driver.Url.Contains(filterQuery))
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
    
    public async Task SolutionKeyAutoProbegAsync(
        HttpRequestMessage httpRequestMessage, 
        CancellationToken stoppingToken = default)
    {
        using var driver = _webDriverService.CreateChromeDriver();
        driver.Navigate().GoToUrl(httpRequestMessage.RequestUri);

        await _webDriverService.FindElementByIdAsync(
            "app", stoppingToken: stoppingToken);

        SetNewCookies(httpRequestMessage, driver.Manage().Cookies.AllCookies);
    }

    public async Task SolutionDromAsync(
        HttpRequestMessage request, 
        CancellationToken stoppingToken = default)
    {
        using var driver = _webDriverService.CreateChromeDriver();
        driver.Navigate().GoToUrl(request.RequestUri?.ToString());

        await _webDriverService.FindElementByCssSelectorAsync(
            "div[data-app-root='bulls-list-auto']", stoppingToken: stoppingToken);

        SetNewCookies(request, driver.Manage().Cookies.AllCookies);
    }

    #region Приватные методы

    private void SetRedirect(
        HttpRequestMessage httpRequest, 
        HttpMethod newMethod, 
        string newUrl)
    {
        Guard.Against.Null(httpRequest, nameof(httpRequest));
        Guard.Against.Null(newMethod, nameof(newMethod));
        Guard.Against.Null(newUrl, nameof(newUrl));
        
        httpRequest.Method = newMethod;
        httpRequest.RequestUri = new Uri(newUrl);
    }

    private void SetAutoRuCookiesAndToken(
        HttpRequestMessage httpRequest, 
        ReadOnlyCollection<Cookie> driverCookies)
    {
        var additionalCookies =
            "yandex_login=max.tkv; Session_id=3:1683636553.5.0.1674329708913:lBY-Lg:31.1.1:czoxNjY4ODg5OTk5NDY5OmxCWS1MZzo0NA.2:1|1370033367.-1.2|385032602.1068.2.2:1068|61:10013073.835889.KOkFiqyyTGSqqXv5GqmuUm-8qMg; " +
            "ys=udn.cDptYXgudGt2#c_chck.3875977813; mda2_beacon=1683636553303; " +
            "sso_status=sso.passport.yandex.ru:synchronized; " +
            "_yasc=X23JRT14qotbUdi5k+Ba23UzhUJN6lkNs8ZZdaJROgdeAS95rlxo8Tw/Z4+K2TE=; gradius=0; " +
            "cycada=8jSDnd6bpcmIU+hlcyNrire9a+uIKehYp+cFw1YoWmI=; _ym_d=1683636760; count-visits=4; " +
            "from_lifetime=1683637032901";
        SetNewCookies(httpRequest, driverCookies, additionalCookies);
        httpRequest.Headers.Add("x-csrf-token", driverCookies.Single(x => x.Name == "_csrf_token").Value);
    }
    
    private void SetNewCookies(
        HttpRequestMessage httpRequestMessage, 
        ReadOnlyCollection<Cookie> cookiesAllCookies, 
        string? additionalCookies = null)
    {
        var cookieValue = string.Join("; ", cookiesAllCookies.OrderBy(x => x.Name)
            .Select(cookie => $"{cookie.Name}={cookie.Value}"));
        if (!string.IsNullOrWhiteSpace(additionalCookies))
        {
            cookieValue += $"; {additionalCookies}";
        }
        httpRequestMessage.Headers.Add("Cookie", cookieValue);
    }

    private async Task<byte[]> DownloadImageAsync(
        string imageUrl, 
        CancellationToken stoppingToken = default)
    {
        try
        {
            using (var httpClient = new HttpClient())
            {
                Console.WriteLine($"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@: {imageUrl}");
                var response = await httpClient.GetAsync(imageUrl, stoppingToken);
                response.EnsureSuccessStatusCode();
                
                return await response.Content.ReadAsByteArrayAsync(stoppingToken);   
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
    
    private string GetCaptchaImageUrl(string html)
    {
        string pattern = "<div class=\"AdvancedCaptcha-View\"><img src=\"(.*?)\".*?><\\/div>";
        var match = Regex.Match(html, pattern);
        if (match.Success)
        {
            return match.Groups[1].Value;
        }
        
        throw new CaptchaSolutionsServiceException(CaptchaInvariants.DetectionErrorInHtml.Replace(":html", html));
    }

    #endregion
}