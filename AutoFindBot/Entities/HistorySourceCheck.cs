namespace AutoFindBot.Entities;

public class HistorySourceCheck : BaseEntity
{
    public virtual long UserFilterId { get; set; }
    
    public Source Source { get; set; }
    
    public bool Success { get; set; }
    
    public string? Data { get; set; }
    
    public virtual UserFilter UserFilter { get; set; }
    
    public virtual List<Car> Cars { get; set; }
}