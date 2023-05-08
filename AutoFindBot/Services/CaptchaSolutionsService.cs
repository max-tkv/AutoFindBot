using System.Collections.ObjectModel;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using AutoFindBot.Abstractions;
using AutoFindBot.Abstractions.HttpClients;
using AutoFindBot.Exceptions;
using AutoFindBot.Invariants;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

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

    public async Task SolutionAsync(HttpRequestMessage httpResponseMessage)
    {
        try
        {
            var options = new ChromeOptions();
            options.AddArgument("--headless");
            using (var driver = new ChromeDriver(@"..\AutoFindBot\Resources\WebDrivers\Win", options))
            {
                var showCaptchaPath = httpResponseMessage.RequestUri?.ToString();
                driver.Navigate().GoToUrl(showCaptchaPath);

                var buttonCaptcha = driver.FindElement(By.Id("js-button"));
                buttonCaptcha.Click();
                await Task.Delay(1000);
            
                var buttonConfirmStart = driver.FindElements(By.Id("confirm-button")).FirstOrDefault();
                if (buttonConfirmStart != null)
                {
                    buttonConfirmStart.Click();
                    
                    var driverCookies1 = driver.Manage().Cookies.AllCookies;
                    SetCookiesAndToken(httpResponseMessage, driverCookies1);
                    SetRedirect(httpResponseMessage, driver);
                    
                    return;
                }
                
                var actionPage = driver.PageSource;
                var imageCaptchaUrl = GetCaptchaImageUrl(actionPage);
                var imageCaptcha = await DownloadImageAsync(imageCaptchaUrl);
        
                var captchaId = await _ruCaptchaHttpApiClient.SendCaptchaAsync(imageCaptcha);
                var code = await GetResultCaptchaWithRetriesAsync(captchaId);

                var inputCode = driver.FindElement(By.Id("xuniq-0-1"));
                inputCode.SendKeys(code);
                await Task.Delay(1000);
            
                var button = driver.FindElement(By.CssSelector("button[data-testid='submit']"));
                button.Click();
                await Task.Delay(1000);
            
                var buttonConfirm = driver.FindElement(By.Id("confirm-button"));
                buttonConfirm.Click();
                await Task.Delay(1000);
                
                var driverCookies = driver.Manage().Cookies.AllCookies;
                SetCookiesAndToken(httpResponseMessage, driverCookies);
                SetRedirect(httpResponseMessage, driver);
            }
        }
        catch (Exception e)
        {
            throw new CaptchaSolutionsServiceException(CaptchaInvariants.SolvingError
                .Replace(":errorMessage", e.Message));
        }
    }

    private void SetRedirect(HttpRequestMessage httpRequest, ChromeDriver driver)
    {
        httpRequest.Method = HttpMethod.Post;
        httpRequest.RequestUri = new Uri(driver.Url);
    }

    private void SetCookiesAndToken(HttpRequestMessage httpRequest, ReadOnlyCollection<Cookie> driverCookies)
    {
        var cookieValue = string.Join("; ", driverCookies.Select(cookie => $"{cookie.Name}={cookie.Value}"));
        cookieValue += "; yandex_login=max.tkv; " +
                       "Session_id=3:1683577497.5.0.1674329708913:lBY-Lg:31.1.1:czoxNjY4ODg5OTk5NDY5OmxCWS1MZzo0NA.2:1|1370033367.-1.2|385032602.1068.2.2:1068|61:10013057.285914.XDsSxbOG95auiz5S7pyhApBbG7I; " +
                       "ys=udn.cDptYXgudGt2#c_chck.647594796; " +
                       "mda2_beacon=1683577497563; " +
                       "sso_status=sso.passport.yandex.ru:synchronized; " +
                       "cycada=EEZhwoin9/0I8xM8HNULp7e9a+uIKehYp+cFw1YoWmI=; " +
                       "_ym_d=1683577509; " +
                       "_yasc=5B7NQJ1PAT2M5j8ZMncY++V0zBceLtKRy7+1ZTFNrnLDUl8XiO5xDQdQWUpObaI=; " +
                       "count-visits=2; " +
                       "from_lifetime=1683577753449";
        httpRequest.Headers.Add("Cookie", cookieValue);
        httpRequest.Headers.Add("x-csrf-token", driverCookies.Single(x => x.Name == "_csrf_token").Value);
    }

    public async Task<string> GetResultCaptchaWithRetriesAsync(string id, int retryCount = 8, int delaySeconds = 10)
    {
        for (int i = 0; i < retryCount; i++)
        {
            try
            {
                return await _ruCaptchaHttpApiClient.GetResultCaptchaAsync(id);
            }
            catch (CaptchaNotReadyException e)
            {
                _logger.LogInformation($"{nameof(CaptchaSolutionsService)}: {e.Message}");
                await Task.Delay(delaySeconds * 1000);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(CaptchaInvariants.RetryExecutionError
                    .Replace(":i", i.ToString())
                    .Replace(":errorMessage", ex.Message));
                await Task.Delay(delaySeconds * 1000);
            }
        }

        throw new CaptchaSolutionsServiceException(CaptchaInvariants.RetryLimitExceeded
            .Replace(":retryCount", retryCount.ToString()));
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