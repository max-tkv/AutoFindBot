namespace AutoFindBot.Utils.Http;

/// <summary>
/// Базовые настройки API клиента
/// </summary>
public class HttpApiClientOptions
{
    /// <summary>
    /// Базовый адрес сервиса
    /// </summary>
    public virtual string BaseUrl { get; set; }
    
    /// <summary>
    /// Host
    /// </summary>
    public virtual string Host { get; set; }

    /// <summary>
    /// Http timeout
    /// </summary>
    public virtual TimeSpan Timeout { get; set; } = TimeSpan.FromMinutes(3);
    
    /// <summary>
    /// Token
    /// </summary>
    public virtual string Token { get; set; }
}