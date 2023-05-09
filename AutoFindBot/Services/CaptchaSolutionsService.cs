using System.Collections.ObjectModel;
using System.Net.Sockets;
using System.Runtime.InteropServices;
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
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-dev-shm-usage");

            var chromeDriverDirectory = @"..\AutoFindBot\Resources\WebDrivers\Win";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                chromeDriverDirectory = "/usr/local/bin";
            }
            using (var driver = new ChromeDriver(chromeDriverDirectory, options))
            {
                var showCaptchaPath = httpResponseMessage.RequestUri?.ToString();
                driver.Navigate().GoToUrl(showCaptchaPath);
                await Task.Delay(1000);

                var buttonAllCookies = driver.FindElements(By.CssSelector("button[data-id='button-all']")).FirstOrDefault();
                if (buttonAllCookies != null)
                {
                    buttonAllCookies.Click();
                    await Task.Delay(1000);
                }
                
                var buttonCaptcha = driver.FindElements(By.Id("js-button")).FirstOrDefault();
                if (buttonCaptcha != null)
                {
                    buttonCaptcha.Click();
                    await Task.Delay(2000);
                }

                var buttonConfirmStart = driver.FindElements(By.Id("confirm-button")).FirstOrDefault();
                if (buttonConfirmStart != null)
                {
                    buttonConfirmStart.Click();
                    
                    SetCookiesAndToken(httpResponseMessage, driver.Manage().Cookies.AllCookies);
                    SetRedirect(httpResponseMessage, driver);
                    
                    return;
                }
                
                var actionPage = driver.PageSource;
                var imageCaptchaUrl = GetCaptchaImageUrl(actionPage);
                var imageCaptcha = await DownloadImageAsync(imageCaptchaUrl);
        
                var captchaId = await _ruCaptchaHttpApiClient.SendCaptchaAsync(imageCaptcha);
                var code = await GetResultCaptchaWithRetriesAsync(captchaId);

                var inputCode = driver.FindElements(By.Id("xuniq-0-1")).FirstOrDefault();
                if (inputCode == null)
                {
                    throw new Exception("Not found 'xuniq-0-1': " + driver.PageSource);
                }
                inputCode?.SendKeys(code);
                await Task.Delay(1000);
            
                var button = driver.FindElements(By.CssSelector("button[data-testid='submit']")).FirstOrDefault();
                if (button == null)
                {
                    throw new Exception("Not found 'button[data-testid='submit']': " + driver.PageSource);
                }
                button?.Click();
                await Task.Delay(1000);
            
                var buttonConfirm = driver.FindElements(By.Id("confirm-button")).FirstOrDefault();
                if (buttonConfirm == null)
                {
                    throw new Exception("Not found 'confirm-button']': " + driver.PageSource);
                }
                buttonConfirm?.Click();
                await Task.Delay(1000);
                
                SetCookiesAndToken(httpResponseMessage, driver.Manage().Cookies.AllCookies);
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
        cookieValue += "; yandex_login=max.tkv; Session_id=3:1683636553.5.0.1674329708913:lBY-Lg:31.1.1:czoxNjY4ODg5OTk5NDY5OmxCWS1MZzo0NA.2:1|1370033367.-1.2|385032602.1068.2.2:1068|61:10013073.835889.KOkFiqyyTGSqqXv5GqmuUm-8qMg; " +
                       "ys=udn.cDptYXgudGt2#c_chck.3875977813; mda2_beacon=1683636553303; " +
                       "sso_status=sso.passport.yandex.ru:synchronized; " +
                       "_yasc=X23JRT14qotbUdi5k+Ba23UzhUJN6lkNs8ZZdaJROgdeAS95rlxo8Tw/Z4+K2TE=; gradius=0; " +
                       "cycada=8jSDnd6bpcmIU+hlcyNrire9a+uIKehYp+cFw1YoWmI=; _ym_d=1683636760; count-visits=4; " +
                       "from_lifetime=1683637032901";
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