using Newtonsoft.Json;

namespace AutoFindBot.Integration.Models;

public class CompanyResponse
{
    [JsonProperty("city")]
    public CityResponse City { get; set; }
}