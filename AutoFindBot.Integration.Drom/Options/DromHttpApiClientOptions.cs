using AutoFindBot.Utils.Http;

namespace AutoFindBot.Integration.Drom.Options;

public class DromHttpApiClientOptions : HttpApiClientOptions
{
    public string GetAutoByFilterQuery { get; set; }
}