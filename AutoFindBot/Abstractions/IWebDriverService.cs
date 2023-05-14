using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace AutoFindBot.Abstractions;

public interface IWebDriverService
{
    ChromeDriver CreateChromeDriver(bool headless = true);

    Task ClickElementByIdAsync(
        string id, 
        bool throwIfNotFound = true, 
        CancellationToken stoppingToken = default);
    
    Task SendKeysByIdAsync(
        string id, 
        string key, 
        bool throwIfNotFound = true, 
        CancellationToken stoppingToken = default);
    
    Task ClickElementByCssSelectorAsync(
        string selector, 
        bool throwIfNotFound = true, 
        CancellationToken stoppingToken = default);

    Task<IWebElement?> FindElementByCssSelectorAsync(
        string selector, 
        bool throwIfNotFound = true, 
        CancellationToken stoppingToken = default);

    Task<IWebElement?> FindElementByIdAsync(
        string id, 
        bool throwIfNotFound = true, 
        CancellationToken stoppingToken = default);
}