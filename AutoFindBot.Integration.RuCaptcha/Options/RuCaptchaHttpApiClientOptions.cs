using AutoFindBot.Utils.Http;

namespace AutoFindBot.Integration.RuCaptcha.Options;

public class RuCaptchaHttpApiClientOptions : HttpApiClientOptions
{
    public string ApiKey { get; set; }
    
    public string InQuery { get; set; }
    
    public string ResQuery { get; set; }
}