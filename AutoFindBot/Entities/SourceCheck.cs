namespace AutoFindBot.Entities;

public class SourceCheck : BaseEntity
{
    public virtual long UserFilterId { get; set; }
    
    public Source Source { get; set; }

    public virtual UserFilter UserFilter { get; set; }
}