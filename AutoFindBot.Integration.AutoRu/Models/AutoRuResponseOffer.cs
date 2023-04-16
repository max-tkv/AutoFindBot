using Newtonsoft.Json;

namespace AutoFindBot.Integration.AutoRu.Models;

public class AutoRuResponseOffer
{
    [JsonProperty("id")]
    public string Id { get; set; }
    
    [JsonProperty("saleId")]
    public string SaleId { get; set; }
    
    [JsonProperty("status")]
    public string Status { get; set; }
    
    [JsonProperty("documents")]
    public AutoRuResponseOfferDocuments Documents { get; set; }
        
    [JsonProperty("price_info")]
    public AutoRuResponseOfferPriceInfo PriceInfo { get; set; }
    
    [JsonProperty("vehicle_info")]
    public AutoRuResponseOfferVehicleInfo VehicleInfo { get; set; }
    
    [JsonProperty("state")]
    public AutoRuResponseOfferState State { get; set; }
    
    [JsonProperty("seller")]
    public AutoRuResponseOfferSeller Seller { get; set; }
}