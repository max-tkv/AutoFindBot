using AutoFindBot.Lookups;

namespace AutoFindBot.Entities;

public class Source : BaseEntity
{
    public string Name { get; set; }
    
    public SourceType SourceType { get; set; }
    
    public bool Active { get; set; }
}