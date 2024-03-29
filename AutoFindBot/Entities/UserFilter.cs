﻿namespace AutoFindBot.Entities;

public class UserFilter : BaseEntity
{
    public virtual long UserId { get; set; }
    public string Title { get; set; }
    public decimal PriceMin { get; set; }
    public decimal PriceMax { get; set; }
    public int YearMin { get; set; }
    public int YearMax { get; set; }
    
    public virtual List<SourceCheck> SourceChecks { get; set; }
    public virtual AppUser User { get; set; }
    public virtual List<Car> Cars { get; set; }
}