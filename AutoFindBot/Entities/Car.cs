namespace AutoFindBot.Entities;

public class Car : BaseEntity
{
    public virtual long UserId { get; set; }
    
    public string Title { get; set; }
    
    public Source Source { get; set; }
    
    public int Year { get; set; }
    
    public int Price { get; set; }

    public string? Vin { get; set; }
    
    public virtual AppUser User { get; set; }
}