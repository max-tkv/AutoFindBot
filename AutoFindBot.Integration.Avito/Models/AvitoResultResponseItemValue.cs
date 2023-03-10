using Newtonsoft.Json;

namespace AutoFindBot.Integration.Avito.Models;

public class AvitoResultResponseItemValue
{
    [JsonProperty("id")]
    public object Id { get; set; }

    [JsonProperty("time")]
    public double Time { get; set; }

    [JsonProperty("title")]
    public string Title { get; set; }

    [JsonProperty("price")]
    public string Price { get; set; }

    [JsonProperty("isFavorite")]
    public bool IsFavorite { get; set; }

    [JsonProperty("location")]
    public string Location { get; set; }

    [JsonProperty("address")]
    public string Address { get; set; }
    
    [JsonProperty("uri_mweb")]
    public string UriMweb { get; set; }
}