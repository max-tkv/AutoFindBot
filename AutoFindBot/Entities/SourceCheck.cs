using AutoFindBot.Lookups;

namespace AutoFindBot.Entities;

public class SourceCheck : BaseEntity
{
    public virtual long UserFilterId { get; set; }
    
    public SourceType SourceType { get; set; }

    public virtual UserFilter UserFilter { get; set; }
}