namespace AutoFindBot.Models.AutoRu;

public class AutoRuResultOffer
{
    public string Id { get; set; }
    
    public string SaleId { get; set; }

    public string Status { get; set; }
    
    public AutoRuResultOfferDocuments Documents { get; set; }
    
    public AutoRuResultOfferPriceInfo PriceInfo { get; set; }
    
    public AutoRuResultOfferVehicleInfo VehicleInfo { get; set; }
    
    public AutoRuResultOfferState State { get; set; }
    
    public AutoRuResultOfferSeller Seller { get; set; }
}