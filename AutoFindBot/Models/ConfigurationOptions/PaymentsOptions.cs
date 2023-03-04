namespace AutoFindBot.Models.ConfigurationOptions;

public class PaymentsOptions : BaseOptions
{
    public override string Name => "Payments";
    
    public bool Active { get; set; }
    public string Token { get; set; }
    public long ShopId { get; set; }
    public long ShopArticleId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Currency { get; set; }
    public Price Price { get; set; }
    public int MaxFreeNumberRequests { get; set; }
}

public class Price
{
    public int Amount { get; set; }
    public string Label { get; set; }
}