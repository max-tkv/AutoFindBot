using Newtonsoft.Json;

namespace AutoFindBot.Integration.Models.TradeDealerClient;

public class CompanyResponse
{
    [JsonProperty("city")]
    public СityResponse City { get; set; }
}