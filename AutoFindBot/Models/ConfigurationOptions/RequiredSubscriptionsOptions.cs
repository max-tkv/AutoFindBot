namespace AutoFindBot.Models.ConfigurationOptions;

public class RequiredSubscriptionsOptions : BaseOptions
{
    public override string Name => "RequiredSubscriptions";
    
    public bool Active { get; set; }
    public List<Group> Groups { get; set; }
}

public class Group 
{
    public string Title { get; set; }
    public string Id { get; set; }
}