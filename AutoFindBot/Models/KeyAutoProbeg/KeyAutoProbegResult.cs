namespace AutoFindBot.Models.KeyAutoProbeg;

public class KeyAutoProbegResult : BaseCarInfo
{
    public string OriginId { get; set; }
    
    public string Title { get; set; }
    
    public string Сity { get; set; }

    public int Year { get; set; }
    
    public int Price { get; set; }

    public string? Vin { get; set; }
    
    public string Url { get; set; }
    
    public DateTime PublishedAt { get; set; }
}