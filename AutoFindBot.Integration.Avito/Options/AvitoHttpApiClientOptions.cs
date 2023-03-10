using AutoFindBot.Utils.Http;

namespace AutoFindBot.Integration.Avito.Options;

public class AvitoHttpApiClientOptions : HttpApiClientOptions
{
    public string Host { get; set; }
    
    public string GetAutoByFilterQuery { get; set; }

    public ProxyData ProxyData { get; set; }
}

public class ProxyData
{
    public bool Active { get; set; }
    
    public string Proxy { get; set; }
    
    public string Login { get; set; }
    
    public string Password { get; set; }
}