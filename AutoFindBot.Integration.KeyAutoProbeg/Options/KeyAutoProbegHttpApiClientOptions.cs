using AutoFindBot.Utils.Http;

namespace AutoFindBot.Integration.KeyAutoProbeg.Options;

public class KeyAutoProbegHttpApiClientOptions : HttpApiClientOptions
{
    public string Host { get; set; }
    
    public string GetAutoByFilterQuery { get; set; }
}