﻿using System.Runtime.InteropServices;
using AutoFindBot.Abstractions;
using AutoFindBot.Exceptions;
using AutoFindBot.Invariants;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace AutoFindBot.Services;

public class WebDriverService : IWebDriverService, IDisposable
{
    private ChromeDriver _driver;

    public ChromeDriver CreateChromeDriver(bool headless = true)
    {
        var options = GetChromeOptions(headless);
        var chromeDriverDirectory = GetChromeDriverDirectory();
        _driver = new ChromeDriver(chromeDriverDirectory, options);

        return _driver;
    }

    public async Task ClickElementByIdAsync(string id, bool throwIfNotFound = true)
    {
        for (int i = 0; i < 3; i++)
        {
            try
            {
                var element = _driver
                    .FindElements(By.Id(id))
                    .Single();
                
                element.Click();
                return;
            }
            catch (InvalidOperationException)
            {
                await Task.Delay(1000);
            }
        }

        if (throwIfNotFound)
        {
            throw new NotFoundWebElementException(WebDriverInvariants.NotFoundWebElementMessage
                .Replace(":element", $"Id = {id}")
                .Replace(":html", _driver.PageSource));   
        }
    }

    public async Task SendKeysByIdAsync(string id, string key, bool throwIfNotFound = true)
    {
        for (int i = 0; i < 3; i++)
        {
            try
            {
                var element = _driver
                    .FindElements(By.Id(id))
                    .Single();
                
                element.SendKeys(key);
                return;
            }
            catch (InvalidOperationException)
            {
                await Task.Delay(1000);
            }
        }

        if (throwIfNotFound)
        {
            throw new NotFoundWebElementException(WebDriverInvariants.NotFoundWebElementMessage
                .Replace(":element", $"Id = {id}")
                .Replace(":html", _driver.PageSource));   
        }
    }

    public async Task ClickElementByCssSelectorAsync(string selector, bool throwIfNotFound = true)
    {
        for (int i = 0; i < 3; i++)
        {
            try
            {
                var element = _driver
                    .FindElements(By.CssSelector(selector))
                    .Single();
                
                element.Click();
                return;
            }
            catch (InvalidOperationException)
            {
                await Task.Delay(1000);
            }
        }

        if (throwIfNotFound)
        {
            throw new NotFoundWebElementException(WebDriverInvariants.NotFoundWebElementMessage
                .Replace(":element", $"CssSelector = {selector}")
                .Replace(":html", _driver.PageSource));   
        }
    }
    
    public async Task<IWebElement?> FindElementByCssSelectorAsync(string selector, bool throwIfNotFound = true)
    {
        for (int i = 0; i < 3; i++)
        {
            try
            {
                return _driver
                    .FindElements(By.CssSelector(selector))
                    .Single();
            }
            catch (InvalidOperationException)
            {
                await Task.Delay(1000);
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
    
    public async Task<IWebElement?> FindElementByIdAsync(string id, bool throwIfNotFound = true)
    {
        for (int i = 0; i < 3; i++)
        {
            try
            {
                return _driver
                    .FindElements(By.Id(id))
                    .Single();
            }
            catch (InvalidOperationException)
            {
                await Task.Delay(1000);
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