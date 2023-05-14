using AutoFindBot.Utils.Http;

namespace AutoFindBot.Integration.Youla.Options;

public class YoulaHttpApiClientOptions : HttpApiClientOptions
{
    public string SiteUrl  { get; set; }
    
    public string GetAutoByFilterQuery { get; set; }
}