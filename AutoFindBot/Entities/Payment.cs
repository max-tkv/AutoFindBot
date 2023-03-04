namespace AutoFindBot.Entities;

public class Payment : BaseEntity
{
    public virtual long UserId { get; set; }
    
    public string PayId { get; set; }
    public string Payload { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public string? OptionId { get; set; }
    public string Currency { get; set; }
    public int Amount { get; set; }
    
    public virtual AppUser User { get; set; }
}