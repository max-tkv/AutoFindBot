using Newtonsoft.Json;

namespace AutoFindBot.Integration.Models;

public class CompanyResponse
{
    [JsonProperty("city")]
    public СityResponse City { get; set; }
}