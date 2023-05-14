using Newtonsoft.Json;

namespace AutoFindBot.Integration.Youla.Models;

public class YoulaRequest
{
    [JsonProperty("operationName")]
    public string OperationName { get; set; }

    [JsonProperty("variables")]
    public YoulaRequestVariables Variables { get; set; }

    [JsonProperty("extensions")]
    public YoulaRequestExtensions Extensions { get; set; }
}