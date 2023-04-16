using Newtonsoft.Json;

namespace AutoFindBot.Integration.AutoRu.Models;

public class AutoRuResponseOfferState
{
    [JsonProperty("mileage")]
    public int Mileage { get; set; }
    
    [JsonProperty("image_urls")]
    public List<AutoRuResponseOfferStateImageUrl> ImageUrls { get; set; }
}