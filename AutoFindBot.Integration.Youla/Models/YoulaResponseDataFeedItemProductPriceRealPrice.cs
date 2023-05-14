using Newtonsoft.Json;

namespace AutoFindBot.Integration.Youla.Models;

public class YoulaResponseDataFeedItemProductPriceRealPrice
{
    [JsonProperty("price")]
    public int Price { get; set; }
}