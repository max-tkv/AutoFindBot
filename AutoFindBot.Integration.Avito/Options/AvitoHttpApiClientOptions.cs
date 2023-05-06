using AutoFindBot.Utils.Http;

namespace AutoFindBot.Integration.Avito.Options;

public class AvitoHttpApiClientOptions : HttpApiClientOptions
{
    public bool Active { get; set; }
    
    public string Host { get; set; }
    
    public string GetAutoByFilterQuery { get; set; }

    public ProxyData ProxyData { get; set; }
    
    public AvitoCookie Cookie { get; set; }
    
    public int DefaultPriceMin { get; set; }
    
    public int DefaultPriceMax { get; set; }
}

public class ProxyData
{
    public bool Active { get; set; }
    
    public string Proxy { get; set; }
    
    public string Login { get; set; }
    
    public string Password { get; set; }
}

public class AvitoCookie
{
    public string U { get; set; }
    
    public string V { get; set; }
}