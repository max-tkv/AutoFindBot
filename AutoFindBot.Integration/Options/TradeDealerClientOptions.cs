using AutoFindBot.Utils.Http;

namespace AutoFindBot.Integration.Options;

public class TradeDealerClientOptions : HttpApiClientOptions
{
    public string Host { get; set; }
}