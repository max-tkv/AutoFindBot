﻿namespace AutoFindBot.Entities;

public class Car : BaseEntity
{
    public virtual long UserId { get; set; }
    
    public virtual long UserFilterId { get; set; }
    
    public virtual long HistorySourceCheckId { get; set; }
    
    public string OriginId { get; set; }
    
    public string Title { get; set; }
    
    public string Сity { get; set; }
    
    public Source Source { get; set; }
    
    public int Year { get; set; }
    
    public int Price { get; set; }

    public string? Vin { get; set; }
    
    public string? Url { get; set; }
    
    public DateTime PublishedAt { get; set; }

    public List<Image> ImageUrls { get; set; } = new List<Image>();
    
    public virtual AppUser User { get; set; }
    
    public virtual UserFilter UserFilter { get; set; }
    
    public virtual HistorySourceCheck HistorySourceCheck { get; set; }
}

public class Image
{
    public Dictionary<string, string> Urls { get; set; }
}