using AutoFindBot.Utils.Http;

namespace AutoFindBot.Integration.Avito.Options;

public class AvitoHttpApiClientOptions : HttpApiClientOptions
{
    public bool Active { get; set; }
    
    public string Host { get; set; }
    
    public string GetAutoByFilterQuery { get; set; }

    public AvitoCookieOptions Cookie { get; set; }
    
    public int DefaultPriceMin { get; set; }
    
    public int DefaultPriceMax { get; set; }
}