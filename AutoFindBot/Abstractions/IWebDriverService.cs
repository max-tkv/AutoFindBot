using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace AutoFindBot.Abstractions;

public interface IWebDriverService
{
    ChromeDriver CreateChromeDriver(bool headless = true);

    Task ClickElementByIdAsync(string id, bool throwIfNotFound = true);
    
    Task SendKeysByIdAsync(string id, string key, bool throwIfNotFound = true);
    
    Task ClickElementByCssSelectorAsync(string selector, bool throwIfNotFound = true);

    Task<IWebElement?> FindElementByCssSelectorAsync(string selector, bool throwIfNotFound = true);

    Task<IWebElement?> FindElementByIdAsync(string id, bool throwIfNotFound = true);
}