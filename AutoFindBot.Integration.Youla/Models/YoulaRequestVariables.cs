using Newtonsoft.Json;

namespace AutoFindBot.Integration.Youla.Models;

public class YoulaRequestVariables
{
    [JsonProperty("sort")]
    public string Sort { get; set; }

    [JsonProperty("attributes")]
    public List<YoulaRequestVariablesAttribute> Attributes { get; set; }

    [JsonProperty("datePublished")]
    public object DatePublished { get; set; }

    [JsonProperty("location")]
    public YoulaRequestVariablesLocation Location { get; set; }

    [JsonProperty("search")]
    public string Search { get; set; } = string.Empty;

    [JsonProperty("cursor")]
    public string Cursor { get; set; } = string.Empty;
}