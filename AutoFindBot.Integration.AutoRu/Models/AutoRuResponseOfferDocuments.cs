using Newtonsoft.Json;

namespace AutoFindBot.Integration.AutoRu.Models;

public class AutoRuResponseOfferDocuments
{
    [JsonProperty("year")]
    public int Year { get; set; }
}