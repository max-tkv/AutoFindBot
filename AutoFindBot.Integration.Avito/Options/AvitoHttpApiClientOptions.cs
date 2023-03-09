using AutoFindBot.Utils.Http;

namespace AutoFindBot.Integration.Avito.Options;

public class AvitoHttpApiClientOptions : HttpApiClientOptions
{
    public string Host { get; set; }
    
    public string GetAutoByFilterQuery { get; set; }
}