using Newtonsoft.Json;

namespace AutoFindBot.Integration.AutoRu.Models;

public class AutoRuCaptchaResponse
{
    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("captcha")]
    public AutoRuCaptchaInfo Captcha { get; set; }
}