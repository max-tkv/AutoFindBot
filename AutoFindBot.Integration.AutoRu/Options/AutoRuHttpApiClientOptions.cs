using AutoFindBot.Utils.Http;

namespace AutoFindBot.Integration.AutoRu.Options;

public class AutoRuHttpApiClientOptions : HttpApiClientOptions
{
    public string GetAutoByFilterQuery { get; set; }
}