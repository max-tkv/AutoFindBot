using Newtonsoft.Json;

namespace AutoFindBot.Integration.AutoRu.Models;

public class AutoRuCaptchaInfo
{
    [JsonProperty("img-url")]
    public string ImgUrl { get; set; }

    [JsonProperty("key")]
    public string Key { get; set; }

    [JsonProperty("status")]
    public string Status { get; set; }

    [JsonProperty("captcha-page")]
    public string CaptchaPage { get; set; }
}