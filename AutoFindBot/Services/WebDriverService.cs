using System.Runtime.InteropServices;
using Ardalis.GuardClauses;
using AutoFindBot.Abstractions;
using AutoFindBot.Exceptions;
using AutoFindBot.Invariants;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace AutoFindBot.Services;

public class WebDriverService : IWebDriverService, IDisposable
{
    private ChromeDriver _driver;
    private readonly ILogger<IWebDriverService> _logger;

    public WebDriverService(ILogger<IWebDriverService> logger)
    {
        _logger = Guard.Against.Null(logger, nameof(logger));
    }
    
    public ChromeDriver CreateChromeDriver(bool headless = true)
    {
        var options = GetChromeOptions(headless);
        var chromeDriverDirectory = GetChromeDriverDirectory();
        _driver = new ChromeDriver(chromeDriverDirectory, options);

        return _driver;
    }

    public async Task ClickElementByIdAsync(
        string id, 
        bool throwIfNotFound = true, 
        CancellationToken stoppingToken = default)
    {
        for (int i = 0; i < 3; i++)
        {
            try
            {
                _logger.LogInformation($"ClickElementByIdAsync for id='{id}'. Retry: {i}.");
                var element = _driver
                    .FindElements(By.Id(id))
                    .Single();

                element.Click();
                return;
            }
            catch (InvalidOperationException)
            {
                _logger.LogInformation($"Page HTML: {_driver.PageSource}");
                await Task.Delay(1000, stoppingToken);
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, $"Необработаное исключение. Message: {e.Message}");
                throw;
            }
        }

        if (throwIfNotFound)
        {
            throw new NotFoundWebElementException(WebDriverInvariants.NotFoundWebElementMessage
                .Replace(":element", $"Id = {id}")
                .Replace(":html", _driver.PageSource));   
        }
    }

    public async Task SendKeysByIdAsync(
        string id, 
        string key, 
        bool throwIfNotFound = true, 
        CancellationToken stoppingToken = default)
    {
        for (int i = 0; i < 3; i++)
        {
            try
            {
                _logger.LogInformation($"SendKeysByIdAsync for id='{id}' key='{key}'. Retry: {i}.");
                var element = _driver
                    .FindElements(By.Id(id))
                    .Single();
                
                element.SendKeys(key);
                return;
            }
            catch (InvalidOperationException)
            {
                _logger.LogInformation($"Page HTML: {_driver.PageSource}");
                await Task.Delay(1000, stoppingToken);
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, $"Необработаное исключение. Message: {e.Message}");
                throw;
            }
        }

        if (throwIfNotFound)
        {
            throw new NotFoundWebElementException(WebDriverInvariants.NotFoundWebElementMessage
                .Replace(":element", $"Id = {id}")
                .Replace(":html", _driver.PageSource));   
        }
    }

    public async Task ClickElementByCssSelectorAsync(
        string selector, 
        bool throwIfNotFound = true, 
        CancellationToken stoppingToken = default)
    {
        for (int i = 0; i < 3; i++)
        {
            try
            {
                _logger.LogInformation($"ClickElementByCssSelectorAsync for selector='{selector}'. Retry: {i}.");
                var element = _driver
                    .FindElements(By.CssSelector(selector))
                    .Single();
                
                element.Click();
                return;
            }
            catch (InvalidOperationException)
            {
                _logger.LogInformation($"Page HTML: {_driver.PageSource}");
                await Task.Delay(1000, stoppingToken);
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, $"Необработаное исключение. Message: {e.Message}");
                throw;
            }
        }

        if (throwIfNotFound)
        {
            throw new NotFoundWebElementException(WebDriverInvariants.NotFoundWebElementMessage
                .Replace(":element", $"CssSelector = {selector}")
                .Replace(":html", _driver.PageSource));   
        }
    }
    
    public async Task<IWebElement?> FindElementByCssSelectorAsync(
        string selector, 
        bool throwIfNotFound = true, 
        CancellationToken stoppingToken = default)
    {
        for (int i = 0; i < 3; i++)
        {
            try
            {
                _logger.LogInformation($"FindElementByCssSelectorAsync for selector='{selector}'. Retry: {i}.");
                return _driver
                    .FindElements(By.CssSelector(selector))
                    .Single();
            }
            catch (InvalidOperationException)
            {
                _logger.LogInformation($"Page HTML: {_driver.PageSource}");
                await Task.Delay(1000, stoppingToken);
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, $"Необработаное исключение. Message: {e.Message}");
                throw;
            }
        }

        if (throwIfNotFound)
        {
            throw new NotFoundWebElementException(WebDriverInvariants.NotFoundWebElementMessage
                .Replace(":element", $"CssSelector = {selector}")
                .Replace(":html", _driver.PageSource));   
        }

        return null;
    }
    
    public async Task<IWebElement?> FindElementByIdAsync(
        string id, 
        bool throwIfNotFound = true, 
        CancellationToken stoppingToken = default)
    {
        for (int i = 0; i < 3; i++)
        {
            try
            {
                _logger.LogInformation($"FindElementByCssSelectorAsync for id='{id}'. Retry: {i}.");
                return _driver
                    .FindElements(By.Id(id))
                    .Single();
            }
            catch (InvalidOperationException)
            {
                _logger.LogInformation($"Page HTML: {_driver.PageSource}");
                await Task.Delay(1000, stoppingToken);
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, $"Необработаное исключение. Message: {e.Message}");
                throw;
            }
        }

        if (throwIfNotFound)
        {
            throw new NotFoundWebElementException(WebDriverInvariants.NotFoundWebElementMessage
                .Replace(":element", $"Id = {id}")
                .Replace(":html", _driver.PageSource));   
        }

        return null;
    }

    private ChromeOptions GetChromeOptions(bool headless)
    {
        var options = new ChromeOptions();
        if (headless)
        {
            options.AddArgument("--headless");
        }
        options.AddArgument("--no-sandbox");
        options.AddArgument("--disable-dev-shm-usage");
        options.AddArgument("--no-cache");
        options.AddArgument("--disable-gpu");
        options.AddArgument("--disable-extensions");

        return options;
    }

    private string GetChromeDriverDirectory()
    {
        var chromeDriverDirectory = @"..\AutoFindBot\Resources\WebDrivers\Win";
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            chromeDriverDirectory = "/usr/local/bin";
        }

        return chromeDriverDirectory;
    }

    public virtual void Dispose()
    {
        _driver.Dispose();
    }
}