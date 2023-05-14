using Newtonsoft.Json;

namespace AutoFindBot.Integration.Youla.Models;

public class YoulaRequestExtensions
{
    [JsonProperty("persistedQuery")]
    public YoulaRequestExtensionsPersistedQuery PersistedQuery { get; set; }
}