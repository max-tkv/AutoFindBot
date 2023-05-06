namespace AutoFindBot.Models.ConfigurationOptions;

public class DefaultFilterOptions : BaseOptions
{
    public override string Name => "DefaultFilter";
    
    public int PriceMin { get; set; }
    
    public int PriceMax { get; set; }
    
    public int YearMin { get; set; }
    
    public int YearMax { get; set; }
}