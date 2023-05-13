using AutoFindBot.Utils.Http;

namespace AutoFindBot.Integration.KeyAutoProbeg.Options;

public class KeyAutoProbegHttpApiClientOptions : HttpApiClientOptions
{
    public string GetAutoByFilterQuery { get; set; }
}