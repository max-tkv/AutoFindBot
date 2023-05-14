using AutoFindBot.Models.ConfigurationOptions;
using Newtonsoft.Json;

namespace AutoFindBot.Integration.Youla.Models;

public class YoulaResponseDataFeedItemProduct
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("price")]
    public YoulaResponseDataFeedItemProductPrice Price { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("images")]
    public List<YoulaResponseDataFeedItemProductImage> Images { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("location")]
    public YoulaResponseDataFeedItemProductLocation Location { get; set; }

    [JsonProperty("distanceText")]
    public string DistanceText { get; set; }
}