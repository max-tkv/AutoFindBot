using Newtonsoft.Json;

namespace AutoFindBot.Integration.Models;

public class TradeDealerResponse
{
    [JsonProperty("total")]
    public int Total { get; set; }

    [JsonProperty("list")]
    public List<CarInfoResponse> CarInfoResponses { get; set; }

    [JsonProperty("mode")]
    public string Mode { get; set; }

    [JsonProperty("canShowMore")]
    public bool CanShowMore { get; set; }

    [JsonProperty("page")]
    public int Page { get; set; }

    [JsonProperty("stockCount")]
    public int StockCount { get; set; }
}