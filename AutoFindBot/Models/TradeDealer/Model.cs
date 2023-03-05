using Newtonsoft.Json;

namespace AutoFindBot.Models.TradeDealer;

public class Model
{
    [JsonProperty("alias")]
    public string Alias { get; set; }
}