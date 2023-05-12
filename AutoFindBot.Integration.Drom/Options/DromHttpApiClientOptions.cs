using AutoFindBot.Utils.Http;

namespace AutoFindBot.Integration.Drom.Options;

public class DromHttpApiClientOptions : HttpApiClientOptions
{
    public bool Active { get; set; }
    
    public string Host { get; set; }
    
    public string GetAutoByFilterQuery { get; set; }
}