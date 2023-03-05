using Newtonsoft.Json;

namespace AutoFindBot.Integration.Models.TradeDealerClient;

public class CompanyResponse
{
    [JsonProperty("title")]
    public string Title { get; set; }
}