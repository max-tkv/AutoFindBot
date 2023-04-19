using AutoFindBot.Utils.Http;

namespace AutoFindBot.Integration.AutoRu.Options;

public class AutoRuHttpApiClientOptions : HttpApiClientOptions
{
    public bool Active { get; set; }
    
    public string GetAutoByFilterQuery { get; set; }
}