namespace AutoFindBot.Models.TradeDealer;

public class CarInfo : BaseCarInfo
{
    public int Year { get; set; }

    public int Price { get; set; }

    public string Vin { get; set; }

    public string Id { get; set; }
    
    public DateTime UpdatedAt { get; set; }
    
    public DateTime PublishedAt { get; set; }
    
    public string Title { get; set; }

    public Company Company { get; set; }
    
    public string VinFull { get; set; }
    
    public Brand Brand { get; set; }
    
    public Generation? Generation { get; set; }
    
    public Model? Model { get; set; }
    
    public List<OriginalPhoto> OriginalPhotos { get; set; }
}