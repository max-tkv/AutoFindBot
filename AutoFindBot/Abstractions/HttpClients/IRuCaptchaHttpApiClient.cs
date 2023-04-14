namespace AutoFindBot.Abstractions.HttpClients;

public interface IRuCaptchaHttpApiClient
{
    public Task<string> SendCaptchaAsync(byte[] file);
    
    public Task<string> GetResultCaptchaAsync(string id);
}