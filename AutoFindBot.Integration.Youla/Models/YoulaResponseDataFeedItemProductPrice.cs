using Newtonsoft.Json;

namespace AutoFindBot.Integration.Youla.Models;

public class YoulaResponseDataFeedItemProductPrice
{
    [JsonProperty("realPrice")]
    public YoulaResponseDataFeedItemProductPriceRealPrice RealPrice { get; set; }
}